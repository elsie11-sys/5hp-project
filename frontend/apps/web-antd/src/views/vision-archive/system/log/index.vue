<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { OperationLogDto } from '#/api/vision-archive/system';

import { computed, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { IconifyIcon } from '@vben/icons';

import {
  Button,
  Descriptions,
  Drawer,
  message,
  Modal,
  Popconfirm,
  Tag,
  Tooltip,
} from 'ant-design-vue';

import { useVbenVxeGrid } from '#/adapter/vxe-table';
import { operationLogApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema, LOG_TYPE_MAP } from './data';

const [Grid, gridApi] = useVbenVxeGrid({
  formOptions: {
    schema: useGridFormSchema(),
    submitOnChange: true,
  },
  gridEvents: {
    checkboxChange: () => {
      selectedRowIds.value = getCheckedRows().map((r: any) => r.id);
    },
    checkboxAll: () => {
      selectedRowIds.value = getCheckedRows().map((r: any) => r.id);
    },
  },
  gridOptions: {
    columns: [
      { type: 'checkbox', width: 50 },
      ...(useColumns() as any[]),
    ],
    height: 'auto',
    keepSource: true,
    showOverflow: true,
    columnConfig: { resizable: true },
    proxyConfig: {
      ajax: {
        query: async ({ page }, formValues = {}) => {
          // RangePicker -> startDate / endDate
          const { createdAtRange, ...rest } = formValues as Record<string, any>;
          const [startDate, endDate] = Array.isArray(createdAtRange)
            ? createdAtRange
            : [undefined, undefined];
          return operationLogApi.getPagedList({
            page: page.currentPage,
            pageSize: page.pageSize,
            ...rest,
            startDate,
            endDate,
          });
        },
      },
    },
    rowConfig: { keyField: 'id' },
    pagerConfig: {
      enabled: true,
      pageSize: 10,
      pageSizes: [10, 20, 50, 100],
      // 对齐设计稿：Total → Sizes → 上下页+页码 → FullJump
      layouts: ['Total', 'Sizes', 'PrevPage', 'JumpNumber', 'NextPage', 'FullJump'],
    },
    toolbarConfig: {
      custom: true,
      export: false,
      refresh: true,
      search: true,
      zoom: true,
    },
  } as VxeTableGridOptions<OperationLogDto>,
});

// ============== 工具栏：批量删除 / 清空 / 导出 ==============
const selectedRowIds = ref<Array<number | string>>([]);
const hasSelection = computed(() => selectedRowIds.value.length > 0);

function getCheckedRows(): OperationLogDto[] {
  return ((gridApi as any).grid?.getCheckboxRecords?.() ?? []) as OperationLogDto[];
}

function confirmDialog(content: string, title: string) {
  return new Promise<void>((resolve, reject) => {
    Modal.confirm({
      content,
      title,
      okType: 'danger',
      onCancel() {
        reject(new Error('cancel'));
      },
      onOk() {
        resolve();
      },
    });
  });
}

async function onBatchDelete() {
  const rows = getCheckedRows();
  if (!rows.length) {
    message.warning('请先勾选要删除的日志');
    return;
  }
  try {
    await confirmDialog(
      `确定要删除选中的 ${rows.length} 条操作日志吗？此操作不可恢复。`,
      '批量删除',
    );
    const ids = rows.map((r) => r.id);
    const res = await operationLogApi.batchDelete(ids);
    message.success(`已删除 ${res.deletedCount} 条日志`);
    selectedRowIds.value = [];
    onRefresh();
  } catch {
    /* 用户取消 */
  }
}

async function onClearAll() {
  try {
    await confirmDialog(
      '确定要清空所有操作日志吗？此操作不可恢复，建议先导出备份。',
      '清空日志',
    );
    await operationLogApi.clear();
    message.success('已清空所有操作日志');
    onRefresh();
  } catch {
    /* 用户取消 */
  }
}

function onExport() {
  // 取当前筛选条件导出
  const formValues = (gridApi as any).formApi?.getValues?.() ?? {};
  const { createdAtRange, ...rest } = formValues as Record<string, any>;
  const [startDate, endDate] = Array.isArray(createdAtRange)
    ? createdAtRange
    : [undefined, undefined];
  operationLogApi.export({ ...rest, startDate, endDate });
  message.success('导出请求已发送');
}

function onRefresh() {
  gridApi.query();
}

// ============== 行内操作：查看详情 ==============
const detailVisible = ref(false);
const detailRow = ref<OperationLogDto | null>(null);

function onView(row: OperationLogDto) {
  detailRow.value = row;
  detailVisible.value = true;
}

// 请求方式 -> 标签颜色（GET 蓝 / POST 绿 / PUT 橙 / DELETE 红，其它走灰）
const METHOD_TAG_COLOR: Record<string, string> = {
  GET: 'blue',
  POST: 'green',
  PUT: 'orange',
  DELETE: 'red',
};

function methodColor(m?: string): string {
  if (!m) return 'default';
  return METHOD_TAG_COLOR[m.toUpperCase()] ?? 'default';
}

// 详情里把 status 翻译成"正常 / 异常"
const detailStatus = computed(() => {
  const s = detailRow.value?.status;
  if (s === 1) return { text: '正常', color: 'green' };
  if (s === 0) return { text: '异常', color: 'red' };
  return { text: '—', color: 'default' };
});

// "操作模块" 显示成 "模块名 / 操作类型"
const detailModule = computed(() => {
  const r = detailRow.value;
  if (!r) return '—';
  const t = LOG_TYPE_MAP[r.type]?.text ?? r.type;
  return r.module ? `${r.module} / ${t}` : t || '—';
});

// 登录信息 = 操作人 / IP / 登录地点
const detailLoginInfo = computed(() => {
  const r = detailRow.value;
  if (!r) return '—';
  return `${r.operator || '—'} / ${r.ip || '—'} / ${r.location || '—'}`;
});

/**
 * 格式化 JSON 字符串：合法 JSON 就 pretty-print，不是就直接原样输出。
 * mock 里有些 JSON 是带 `, 0` 缩进过的（已经压缩），需要重新展开。
 */
function prettyJson(raw: string): string {
  try {
    return JSON.stringify(JSON.parse(raw), null, 2);
  } catch {
    return raw;
  }
}

// ============== 表格列渲染辅助 ==============

/** 操作内容最多展示多少字（超过则用 ... 截断） */
const CONTENT_MAX_LEN = 15;

function truncateContent(text?: string): string {
  if (!text) return '—';
  return text.length > CONTENT_MAX_LEN
    ? `${text.slice(0, CONTENT_MAX_LEN)}…`
    : text;
}

const isContentOverflow = (text?: string) =>
  !!text && text.length > CONTENT_MAX_LEN;
</script>

<template>
  <Page auto-content-height>
    <Grid table-title="操作日志">
      <template #toolbar-tools>
        <div class="flex items-center gap-2">
          <Button
            class="btn-danger-outline"
            :disabled="!hasSelection"
            @click="onBatchDelete"
          >
            <IconifyIcon icon="lucide:trash-2" class="mr-1 size-4" />
            删除
          </Button>
          <Popconfirm
            title="确认清空所有操作日志?"
            ok-type="danger"
            @confirm="onClearAll"
          >
            <Button class="btn-danger-outline">
              <IconifyIcon icon="lucide:eraser" class="mr-1 size-4" />
              清空
            </Button>
          </Popconfirm>
          <Button class="btn-warning-outline" @click="onExport">
            <IconifyIcon icon="lucide:download" class="mr-1 size-4" />
            导出
          </Button>
        </div>
      </template>

      <template #action="{ row }">
        <Button
          size="small"
          type="link"
          title="查看详情"
          @click="onView(row)"
        >
          <IconifyIcon icon="lucide:eye" class="size-4" />
        </Button>
      </template>

      <!-- 操作内容：截 15 字 + ...，hover 显示全量 -->
      <template #content="{ row }">
        <Tooltip
          v-if="isContentOverflow(row.content)"
          :title="row.content"
          placement="topLeft"
        >
          <span class="log-content-cell">{{ truncateContent(row.content) }}</span>
        </Tooltip>
        <span v-else class="log-content-cell">{{ row.content || '—' }}</span>
      </template>
    </Grid>

    <!-- 详情抽屉 -->
    <Drawer
      v-model:open="detailVisible"
      title="操作日志详情"
      :width="720"
      :footer-style="{ textAlign: 'right' }"
    >
      <template v-if="detailRow">
        <Descriptions
          :column="2"
          size="small"
          bordered
          :label-style="{ width: '100px', color: 'rgba(0,0,0,0.65)' }"
        >
          <Descriptions.Item label="操作模块">
            {{ detailModule }}
          </Descriptions.Item>
          <Descriptions.Item label="请求地址">
            <span class="log-code-inline">{{ detailRow.requestUrl || '—' }}</span>
          </Descriptions.Item>

          <Descriptions.Item label="登录信息">
            {{ detailLoginInfo }}
          </Descriptions.Item>
          <Descriptions.Item label="请求方式">
            <Tag :color="methodColor(detailRow.requestMethod)">
              {{ detailRow.requestMethod || '—' }}
            </Tag>
          </Descriptions.Item>

          <Descriptions.Item label="操作方法" :span="2">
            <span class="log-code-inline">{{ detailRow.method || '—' }}</span>
          </Descriptions.Item>

          <Descriptions.Item label="请求参数" :span="2">
            <pre class="log-code-block">{{
              detailRow.requestParams
                ? prettyJson(detailRow.requestParams)
                : '—'
            }}</pre>
          </Descriptions.Item>

          <Descriptions.Item label="返回参数" :span="2">
            <pre class="log-code-block">{{
              detailRow.responseParams
                ? prettyJson(detailRow.responseParams)
                : '—'
            }}</pre>
          </Descriptions.Item>

          <!-- 底部三列：操作状态 / 消耗时间 / 操作时间，用 flex 模拟 3 列 -->
          <Descriptions.Item :span="2">
            <div class="log-status-row">
              <div class="log-status-cell">
                <span class="log-status-label">操作状态：</span>
                <Tag :color="detailStatus.color">{{ detailStatus.text }}</Tag>
              </div>
              <div class="log-status-cell">
                <span class="log-status-label">消耗时间：</span>
                <span>{{ detailRow.costMs == null ? '—' : `${detailRow.costMs}毫秒` }}</span>
              </div>
              <div class="log-status-cell">
                <span class="log-status-label">操作时间：</span>
                <span>{{ detailRow.createdAt }}</span>
              </div>
            </div>
          </Descriptions.Item>
        </Descriptions>
      </template>
      <template #footer>
        <Button @click="detailVisible = false">关闭</Button>
      </template>
    </Drawer>
  </Page>
</template>

<style scoped>
/* ============== 危险按钮（红色边框白底） ============== */
.btn-danger-outline {
  color: #ff4d4f;
  border-color: #ffa39e;
  background: #fff1f0;
}
.btn-danger-outline:hover,
.btn-danger-outline:focus {
  color: #cf1322;
  border-color: #ff4d4f;
  background: #ffccc7;
}
.btn-danger-outline:disabled,
.btn-danger-outline:disabled:hover,
.btn-danger-outline:disabled:focus {
  color: rgba(0, 0, 0, 0.25);
  border-color: #d9d9d9;
  background: #f5f5f5;
}

/* ============== 警告按钮（橙色边框白底，对齐设计稿的"导出"） ============== */
.btn-warning-outline {
  color: #d46b08;
  border-color: #ffd591;
  background: #fff7e6;
}
.btn-warning-outline:hover,
.btn-warning-outline:focus {
  color: #ad4e00;
  border-color: #fa8c16;
  background: #ffe7ba;
}

/* ============== 表格：操作内容（截断 + 单行省略号） ============== */
.log-content-cell {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  vertical-align: middle;
  color: #303133;
  font-size: 13px;
}

/* ============== 详情：行内 code（请求地址 / 操作方法） ============== */
.log-code-inline {
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
  font-size: 12.5px;
  color: #303133;
  word-break: break-all;
}

/* ============== 详情：JSON 块（请求参数 / 返回参数） ============== */
.log-code-block {
  margin: 0;
  padding: 10px 12px;
  background: #fafafa;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
  font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
  font-size: 12.5px;
  line-height: 1.6;
  color: #303133;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 240px;
  overflow: auto;
}

/* ============== 详情：底部三列状态行 ============== */
.log-status-row {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 2px 0;
  gap: 16px;
}
.log-status-cell {
  flex: 1 1 0;
  min-width: 0;
  display: flex;
  align-items: center;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.log-status-cell:last-child {
  flex: 1.4 1 0;
  justify-content: flex-end;
  text-align: right;
}
.log-status-label {
  color: rgba(0, 0, 0, 0.65);
  margin-right: 4px;
  flex-shrink: 0;
}

/* ============== 分页器样式：antd 默认浅色风格 + 靠右对齐 ============== */
:deep(.vxe-pager) {
  background: #fff !important;
  border-top: 1px solid #f0f0f0 !important;
  border-radius: 0 !important;
  padding: 12px 16px !important;
  min-height: auto !important;
  font-size: 13px !important;
  color: rgba(0, 0, 0, 0.65) !important;
  display: flex !important;
  justify-content: flex-end !important;
  align-items: center !important;
  gap: 8px !important;
}
:deep(.vxe-pager .vxe-pager--total) {
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
  margin-right: 0 !important;
}
:deep(.vxe-pager .vxe-pager--btn) {
  background: #f5f5f5 !important;
  border: 1px solid transparent !important;
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
  min-width: 32px !important;
  height: 30px !important;
  margin: 0 !important;
  border-radius: 4px !important;
  padding: 0 8px !important;
  transition: all 0.2s ease !important;
}
:deep(.vxe-pager .vxe-pager--btn:hover) {
  color: #52c41a !important;
  background: #f5f5f5 !important;
  border-color: transparent !important;
}
:deep(.vxe-pager .vxe-pager--btn.is--active) {
  background: #52c41a !important;
  border-color: #52c41a !important;
  color: #fff !important;
  font-weight: 500 !important;
}
:deep(.vxe-pager .vxe-pager--btn.is--disabled) {
  color: rgba(0, 0, 0, 0.25) !important;
  cursor: not-allowed !important;
  background: #f5f5f5 !important;
  border-color: transparent !important;
}
:deep(.vxe-pager .vxe-pager--sizes) {
  margin: 0 !important;
}
:deep(.vxe-pager .vxe-pager--sizes select),
:deep(.vxe-pager .vxe-pager--jump input) {
  background: #fff !important;
  border: 1px solid #d9d9d9 !important;
  color: rgba(0, 0, 0, 0.85) !important;
  height: 30px !important;
  border-radius: 4px !important;
  font-size: 13px !important;
}
:deep(.vxe-pager .vxe-pager--jump) {
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
}
:deep(.vxe-pager .vxe-pager--prev-btn),
:deep(.vxe-pager .vxe-pager--next-btn) {
  background: #f5f5f5 !important;
  color: rgba(0, 0, 0, 0.65) !important;
  border: 1px solid transparent !important;
  height: 30px !important;
  width: 32px !important;
  border-radius: 4px !important;
  padding: 0 !important;
}
:deep(.vxe-pager .vxe-pager--prev-btn:hover),
:deep(.vxe-pager .vxe-pager--next-btn:hover) {
  color: #52c41a !important;
  background: #f5f5f5 !important;
}
:deep(.vxe-pager .vxe-pager--prev-btn.is--disabled),
:deep(.vxe-pager .vxe-pager--next-btn.is--disabled) {
  color: rgba(0, 0, 0, 0.25) !important;
  cursor: not-allowed !important;
  background: #f5f5f5 !important;
}
</style>
