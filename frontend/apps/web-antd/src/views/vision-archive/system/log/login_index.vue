<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { LoginLogDto } from '#/api/vision-archive/system';

import { computed, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { IconifyIcon } from '@vben/icons';

import {
  Button,
  Input,
  message,
  Modal,
  Popconfirm,
  Tag,
  Tooltip,
} from 'ant-design-vue';

import { useVbenVxeGrid } from '#/adapter/vxe-table';
import { loginLogApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema } from './login_data';

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
          const { loginTimeRange, ...rest } = formValues as Record<string, any>;
          const [startDate, endDate] = Array.isArray(loginTimeRange)
            ? loginTimeRange
            : [undefined, undefined];
          return loginLogApi.getPagedList({
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
  } as VxeTableGridOptions<LoginLogDto>,
});

// ============== 工具栏：批量删除 / 清空 / 解锁 / 导出 ==============
const selectedRowIds = ref<Array<number | string>>([]);
const hasSelection = computed(() => selectedRowIds.value.length > 0);

function getCheckedRows(): LoginLogDto[] {
  return ((gridApi as any).grid?.getCheckboxRecords?.() ?? []) as LoginLogDto[];
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
      `确定要删除选中的 ${rows.length} 条登录日志吗？此操作不可恢复。`,
      '批量删除',
    );
    const ids = rows.map((r) => r.id);
    const res = await loginLogApi.batchDelete(ids);
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
      '确定要清空所有登录日志吗？此操作不可恢复，建议先导出备份。',
      '清空日志',
    );
    await loginLogApi.clear();
    message.success('已清空所有登录日志');
    onRefresh();
  } catch {
    /* 用户取消 */
  }
}

/**
 * 解锁账户：弹窗输入要解锁的用户名，提交后端接口解锁。
 * 后端未就绪时走 mock 返回成功。
 */
const unlockVisible = ref(false);
const unlockUserName = ref('');

function openUnlockDialog() {
  unlockUserName.value = '';
  unlockVisible.value = true;
}

async function onConfirmUnlock() {
  const name = unlockUserName.value.trim();
  if (!name) {
    message.warning('请输入要解锁的用户名');
    return;
  }
  try {
    const res = await loginLogApi.unlock(name);
    message.success(res.message || `账户 [${name}] 已解锁`);
    unlockVisible.value = false;
  } catch (err: any) {
    message.error(err?.message || '解锁失败');
  }
}

function onExport() {
  // 取当前筛选条件导出
  const formValues = (gridApi as any).formApi?.getValues?.() ?? {};
  const { loginTimeRange, ...rest } = formValues as Record<string, any>;
  const [startDate, endDate] = Array.isArray(loginTimeRange)
    ? loginTimeRange
    : [undefined, undefined];
  loginLogApi.export({ ...rest, startDate, endDate });
  message.success('导出请求已发送');
}

function onRefresh() {
  gridApi.query();
}

// ============== 表格列渲染辅助 ==============

/** 描述最多展示多少字（超过则用 ... 截断） */
const MESSAGE_MAX_LEN = 18;

function truncateMessage(text?: string): string {
  if (!text) return '—';
  return text.length > MESSAGE_MAX_LEN
    ? `${text.slice(0, MESSAGE_MAX_LEN)}…`
    : text;
}

const isMessageOverflow = (text?: string) =>
  !!text && text.length > MESSAGE_MAX_LEN;
</script>

<template>
  <Page auto-content-height>
    <Grid table-title="登录日志">
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
            title="确认清空所有登录日志?"
            ok-type="danger"
            @confirm="onClearAll"
          >
            <Button class="btn-danger-outline">
              <IconifyIcon icon="lucide:eraser" class="mr-1 size-4" />
              清空
            </Button>
          </Popconfirm>
          <Button class="btn-success-outline" @click="openUnlockDialog">
            <IconifyIcon icon="lucide:unlock" class="mr-1 size-4" />
            解锁
          </Button>
          <Button class="btn-warning-outline" @click="onExport">
            <IconifyIcon icon="lucide:download" class="mr-1 size-4" />
            导出
          </Button>
        </div>
      </template>

      <!-- 描述：截 18 字 + ...，hover 显示全量 -->
      <template #message="{ row }">
        <Tooltip
          v-if="isMessageOverflow(row.message)"
          :title="row.message"
          placement="topLeft"
        >
          <span class="log-cell-text">{{ truncateMessage(row.message) }}</span>
        </Tooltip>
        <span v-else class="log-cell-text">{{ row.message || '—' }}</span>
      </template>

      <!-- 登录状态：用绿色 Tag，与设计稿一致 -->
      <template #status="{ row }">
        <Tag :color="row.status === 1 ? 'green' : 'red'">
          {{ row.status === 1 ? '成功' : '失败' }}
        </Tag>
      </template>
    </Grid>

    <!-- 账户解锁弹窗 -->
    <Modal
      v-model:open="unlockVisible"
      title="账户解锁"
      :width="420"
      ok-text="解锁"
      @ok="onConfirmUnlock"
    >
      <div class="unlock-form">
        <span class="unlock-form-label">用户名：</span>
        <Input
          v-model:value="unlockUserName"
          placeholder="请输入要解锁的用户名"
          allow-clear
        />
      </div>
    </Modal>
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

/* ============== 成功按钮（绿色边框白底，对齐设计稿的"解锁"） ============== */
.btn-success-outline {
  color: #389e0d;
  border-color: #b7eb8f;
  background: #f6ffed;
}
.btn-success-outline:hover,
.btn-success-outline:focus {
  color: #237804;
  border-color: #52c41a;
  background: #d9f7be;
}

/* ============== 表格：描述（截断 + 单行省略号） ============== */
.log-cell-text {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  vertical-align: middle;
  color: #303133;
  font-size: 13px;
}

/* ============== 解锁弹窗：用户名输入行 ============== */
.unlock-form {
  display: flex;
  align-items: center;
  padding: 8px 0;
}
.unlock-form-label {
  width: 70px;
  color: rgba(0, 0, 0, 0.65);
  font-size: 14px;
  flex-shrink: 0;
}
.unlock-form :deep(.ant-input) {
  flex: 1;
}

/* ============== 分页器样式：与原 log/index.vue 保持一致 ============== */
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
