<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { RoleDto as SystemRole } from '#/api/vision-archive/system';

import { computed, ref } from 'vue';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import {
  Button,
  Drawer,
  Form as AntForm,
  FormItem,
  Input,
  message,
  Modal,
  Select,
  Tooltip,
} from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { DATA_SCOPE_OPTIONS, dictApi, roleApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema } from './data';
import Form from './modules/form.vue';

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: Form,
  destroyOnClose: true,
});

// 版本标记：方便确认浏览器拿到的就是最新 bundle（dev tools 控制台搜 "v-role-fix" 即可）
console.info(
  '%c[v-role-fix] CellSwitch activeValue=1/inactiveValue=0 + onStatusChange onRefresh 修复已加载',
  'color:#52c41a;font-weight:bold',
);

// 工具栏批量按钮需要根据勾选状态控制 disabled
const selectedRowIds = ref<Array<number | string>>([]);
const hasSelection = computed(() => selectedRowIds.value.length > 0);
const hasSingleSelection = computed(() => selectedRowIds.value.length === 1);

function confirm(content: string, title: string) {
  return new Promise<void>((resolve, reject) => {
    Modal.confirm({
      content,
      title,
      onCancel() {
        reject(new Error('取消'));
      },
      onOk() {
        resolve();
      },
    });
  });
}

async function onStatusChange(newStatus: number, row: SystemRole) {
  const text = newStatus === 1 ? '启用' : '禁用';
  try {
    await confirm(`确定要${text}角色【${row.name}】吗？`, '状态切换');
    // 后端会校验 URL 中的 id 和 body 中的 id 一致，且必填字段（如 name）不能为空
    // 所以把整行原样带上，再覆盖 status 为新值
    await roleApi.update(row.id, { ...row, id: row.id, status: newStatus } as any);
    // 1) 手动同步 grid 行内 status 字段，避免 CellSwitch 内部 state 与 grid 数据脱节
    row.status = newStatus;
    message.success('状态更新成功');
    // 2) 强制 reload grid，从后端拉最新数据，避免所有行都显示成关
    onRefresh();
    return true;
  } catch {
    return false;
  }
}

function onEdit(row: SystemRole) {
  formDrawerApi.setData(row).open();
}

async function onDelete(row: SystemRole) {
  await confirm(`确定要删除角色【${row.name}】吗？`, '删除确认');
  await roleApi.delete(row.id);
  message.success('删除成功');
  onRefresh();
}

function onRefresh() {
  gridApi.query();
}

function onCreate() {
  formDrawerApi.setData({}).open();
}

// ============== 工具栏按钮：批量修改/删除/导出 ==============
function getCheckedRows(): SystemRole[] {
  // vxe grid 实例方法：获取所有勾选的行
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return ((gridApi as any).grid?.getCheckboxRecords?.() ?? []) as SystemRole[];
}

async function onToolbarEdit() {
  // 按钮已通过 :disabled="!hasSingleSelection" 控制启用，
  // 这里只做一次选中行的兜底校验，避免外部绕过 disabled 触发
  const rows = getCheckedRows();
  if (rows.length !== 1) {
    message.warning('请选择一行后再修改');
    return;
  }
  const row = rows[0];
  if (!row) return; // 兜底：strict 模式下 rows[0] 仍可能被推为 undefined
  onEdit(row);
}

async function onToolbarDelete() {
  const rows = getCheckedRows();
  if (rows.length === 0) return;
  // 过滤掉内置 admin
  const deletable = rows.filter((r) => !isSystemRole(r));
  if (deletable.length === 0) {
    message.warning('选中的角色均为内置系统角色，不允许删除');
    return;
  }
  try {
    await confirm(`确定要删除选中的 ${deletable.length} 个角色吗？`, '删除确认');
    await roleApi.batchDelete(deletable.map((r) => r.id) as number[]);
    message.success(`已删除 ${deletable.length} 个角色`);
    selectedRowIds.value = [];
    onRefresh();
  } catch {
    // 用户取消
  }
}

function onToolbarExport() {
  // 直接调后端 /api/role/export，浏览器下载
  roleApi.export();
  message.success('导出请求已发送');
}

// ============== 数据权限 ==============
// 权限范围下拉选项：来源是 sys_dict 中 DictType=role_based_data_permissions 的字典项
const DATA_SCOPE_DICT_TYPE = 'role_based_data_permissions';
const dataScopeDrawer = ref<{
  open: boolean;
  row: SystemRole | null;
  dataScope: string;
  options: Array<{ label: string; value: string }>;
  loadingDict: boolean;
}>({
  open: false,
  row: null,
  dataScope: 'ALL',
  // 兜底：字典没加载到时用硬编码常量，不让抽屉打不开
  options: DATA_SCOPE_OPTIONS as Array<{ label: string; value: string }>,
  loadingDict: false,
});

async function loadDataScopeDict(): Promise<Array<{ label: string; value: string }>> {
  try {
    const dict = await dictApi.getByType(DATA_SCOPE_DICT_TYPE);
    const items = (dict?.dictItems ?? [])
      .filter((it) => it.status === 1 || it.status === undefined)
      .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
      .map((it) => ({ label: it.itemLabel, value: it.itemValue }));
    return items.length > 0 ? items : (DATA_SCOPE_OPTIONS as Array<{ label: string; value: string }>);
  } catch {
    // 字典接口不可用时兜底为硬编码常量
    return DATA_SCOPE_OPTIONS as Array<{ label: string; value: string }>;
  }
}

/**
 * 把任意来源的 dataScope（可能是 row.dataScope / getDataPermission 返回 / 历史脏值）
 * 归一化为 options 里的一个有效 value。
 * - 优先匹配 itemValue（ALL/CUSTOM/DEPT/DEPT_AND_BELOW/SELF）
 * - 找不到时回退到第一个有效选项
 * 避免出现「select v-model 是脏值 → 用户没点下拉 → 提交脏值」的情况
 */
function resolveDataScopeValue(
  raw: unknown,
  options: Array<{ label: string; value: string }>,
): string {
  const fallback = options[0]?.value ?? 'ALL';
  if (raw === undefined || raw === null || raw === '') return fallback;
  const s = String(raw).trim();
  if (!s) return fallback;
  return options.some((o) => o.value === s) ? s : fallback;
}

async function onAssignDataScope(row: SystemRole) {
  // 打开抽屉时进入 loading：v-model 暂用空串（绝不能是脏值），
  // 等字典项 + 当前值都拉完，归一化后再一次性放开（loadingDict=false）。
  // 这样用户点击「确定」时 v-model 必然是合法 value。
  dataScopeDrawer.value = {
    open: true,
    row,
    dataScope: '',
    options: [],
    loadingDict: true,
  };

  const fallbackOptions = DATA_SCOPE_OPTIONS as Array<{ label: string; value: string }>;
  let options: Array<{ label: string; value: string }> = fallbackOptions;
  let currentDataScope: string | undefined;

  try {
    await Promise.all([
      (async () => {
        try {
          const cur = await roleApi.getDataPermission(row.id);
          if (cur?.dataScope) currentDataScope = String(cur.dataScope);
        } catch {
          /* 拉不到当前值就用 row.dataScope 兜底 */
        }
      })(),
      (async () => {
        options = await loadDataScopeDict();
      })(),
    ]);
  } finally {
    // 一次性把 options + 归一化后的 v-model + loading=false 写回去，
    // 避免中间态被 select 抢渲染
    const sourceValue = currentDataScope ?? row.dataScope;
    dataScopeDrawer.value = {
      ...dataScopeDrawer.value,
      options,
      dataScope: resolveDataScopeValue(sourceValue, options),
      loadingDict: false,
    };
  }
}

async function onConfirmDataScope() {
  const m = dataScopeDrawer.value;
  if (!m.row) return;
  // 强转 String，避免历史脏数据把 number 传出去
  const dataScope = String(m.dataScope ?? '').trim();
  if (!dataScope) {
    message.warning('请选择权限范围');
    return;
  }
  // 兜底：如果字典项里有更标准的值（比如字典没加载到时），用归一化函数再校一遍
  const normalized = resolveDataScopeValue(dataScope, m.options);
  try {
    await roleApi.assignDataPermission(m.row.id, { dataScope: normalized });
    const label =
      m.options.find((o) => o.value === normalized)?.label ?? normalized;
    message.success(`已为【${m.row.name}】分配数据权限：${label}`);
    m.open = false;
    onRefresh();
  } catch {
    message.error('数据权限分配失败');
  }
}

// ============== 分配用户（占位） ==============
function onAssignUsers(row: SystemRole) {
  // TODO: 后端 roleApi 暂无分配用户接口，先占位
  message.warning(`分配用户功能开发中（角色：${row.name}）`);
}

// 内置系统角色（code === 'admin'）不允许任何操作
const isSystemRole = (row: SystemRole) => row.code === 'admin';

// 备注列：截断显示 + hover 提示全量
const REMARK_MAX_LEN = 15;
function truncateRemark(text?: string) {
  if (!text) return '—';
  if (text.length <= REMARK_MAX_LEN) return text;
  return `${text.slice(0, REMARK_MAX_LEN)}…`;
}
const remarkShowFull = computed(() => (text?: string) => (text ?? '').length > REMARK_MAX_LEN);

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
      ...(useColumns({ onStatusChange }) as any[]),
    ],
    height: 'auto',
    keepSource: true,
    proxyConfig: {
      ajax: {
        query: async ({ page }, formValues = {}) => {
          // 把 RangePicker 的 [start, end] 拆成后端要的 startDate / endDate
          const { createdAtRange, ...rest } = formValues as Record<string, any>;
          const [startDate, endDate] = Array.isArray(createdAtRange) ? createdAtRange : [undefined, undefined];
          const result = await roleApi.getPagedList({
            page: page.currentPage,
            pageSize: page.pageSize,
            ...rest,
            startDate,
            endDate,
          });
          // 调试：把 grid 实际拿到的第一条行打出来，方便核对字段名/值
          // dev tools 控制台搜 "v-role-paged" 即可
          // eslint-disable-next-line no-console
          console.info(
            '%c[v-role-paged] grid query result:',
            'color:#1677ff',
            {
              total: (result as any)?.total,
              firstRow: (result as any)?.items?.[0],
              firstRowStatus: (result as any)?.items?.[0]?.status,
            },
          );
          return result;
        },
      },
    },
    rowConfig: {
      keyField: 'id',
    },
    pagerConfig: {
      enabled: true,
      pageSize: 10,
      pageSizes: [10, 20, 50, 100],
      // 顺序对齐设计稿：Total(共 N 条) → Sizes(10条/页) → 上下页 + 页码 → FullJump(前往 N 页)
      layouts: [
        'Total',
        'Sizes',
        'PrevPage',
        'JumpNumber',
        'NextPage',
        'FullJump',
      ],
    },
    toolbarConfig: {
      custom: true,
      export: false,
      refresh: true,
      search: true,
      zoom: true,
    },
  } as VxeTableGridOptions<SystemRole>,
});
</script>
<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <Grid table-title="角色列表">
      <template #toolbar-tools>
        <div class="flex items-center gap-2">
          <Button type="primary" @click="onCreate">
            <Plus class="mr-1 size-4" />
            新增
          </Button>
          <Button
            class="btn-success"
            :disabled="!hasSingleSelection"
            @click="onToolbarEdit"
          >
            <IconifyIcon icon="lucide:pencil" class="mr-1 size-4" />
            修改
          </Button>
          <Button
            class="btn-danger-outline"
            :disabled="!hasSelection"
            @click="onToolbarDelete"
          >
            <IconifyIcon icon="lucide:trash-2" class="mr-1 size-4" />
            删除
          </Button>
          <Button @click="onToolbarExport">
            <IconifyIcon icon="lucide:download" class="mr-1 size-4" />
            导出
          </Button>
        </div>
      </template>

      <!-- 备注列：截断 + hover 显示全量 -->
      <template #remark="{ row }">
        <Tooltip
          v-if="remarkShowFull(row.remark)"
          :title="row.remark"
          placement="topLeft"
        >
          <span class="role-remark-cell">{{ truncateRemark(row.remark) }}</span>
        </Tooltip>
        <span v-else class="role-remark-cell">{{ truncateRemark(row.remark) }}</span>
      </template>

      <template #action="{ row }">
        <VbenTableAction
          :actions="[
            {
              text: '修改',
              icon: 'ant-design:edit-outlined',
              tooltip: '修改',
              onClick: () => onEdit(row),
              ifShow: () => !isSystemRole(row),
            },
            {
              text: '删除',
              icon: 'lucide:trash-2',
              tooltip: '删除',
              danger: true,
              popConfirm: {
                title: '确认删除此角色?',
                confirm: () => onDelete(row),
              },
              ifShow: () => !isSystemRole(row),
            },
            {
              text: '数据权限',
              icon: 'lucide:circle-check',
              tooltip: '数据权限',
              onClick: () => onAssignDataScope(row),
              ifShow: () => !isSystemRole(row),
            },
            {
              text: '分配用户',
              icon: 'lucide:user',
              tooltip: '分配用户',
              onClick: () => onAssignUsers(row),
              ifShow: () => !isSystemRole(row),
            },
          ]"
          align="center"
        />
      </template>
    </Grid>

    <!-- 数据权限分配 Drawer -->
    <Drawer
      v-model:open="dataScopeDrawer.open"
      title="分配数据权限"
      :width="500"
      :footer-style="{ textAlign: 'right' }"
    >
      <AntForm layout="vertical" :colon="false">
        <FormItem label="角色名称">
          <Input
            :value="dataScopeDrawer.row?.name"
            disabled
            placeholder="—"
          />
        </FormItem>
        <FormItem label="权限字符">
          <Input
            :value="dataScopeDrawer.row?.code"
            disabled
            placeholder="—"
          />
        </FormItem>
        <FormItem label="权限范围">
          <Select
            :value="dataScopeDrawer.dataScope"
            :options="dataScopeDrawer.options"
            :loading="dataScopeDrawer.loadingDict"
            :disabled="dataScopeDrawer.loadingDict"
            placeholder="请选择数据权限"
            class="w-full"
            @change="
              (v: any) => {
                // 显式归一化，杜绝任何非法值提交到后端
                const normalized = resolveDataScopeValue(
                  String(v ?? '').trim(),
                  dataScopeDrawer.options,
                );
                dataScopeDrawer.dataScope = normalized;
              }
            "
          />
        </FormItem>
      </AntForm>
      <template #footer>
        <Button @click="dataScopeDrawer.open = false">取消</Button>
        <Button
          type="primary"
          :disabled="dataScopeDrawer.loadingDict"
          @click="onConfirmDataScope"
        >
          确定
        </Button>
      </template>
    </Drawer>
  </Page>
</template>

<style scoped>
/*
 * 分页器样式：antd 默认浅色风格 + 靠右对齐
 */
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
:deep(.vxe-pager .vxe-pager--btn),
:deep(.vxe-pager--num-btn) {
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
:deep(.vxe-pager .vxe-pager--btn:hover),
:deep(.vxe-pager--num-btn:hover) {
  color: #52c41a !important;
  background: #f5f5f5 !important;
  border-color: transparent !important;
}
:deep(.vxe-pager .vxe-pager--btn.is--active),
:deep(.vxe-pager--num-btn.is--active),
:deep(.vxe-pager--num-btn.active) {
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
  border-radius: 4px !important;
  font-size: 13px !important;
  height: 30px !important;
  padding: 0 8px !important;
  transition: all 0.2s ease !important;
}
:deep(.vxe-pager .vxe-pager--sizes select:focus),
:deep(.vxe-pager .vxe-pager--jump input:focus) {
  outline: none !important;
  border-color: #52c41a !important;
}
:deep(.vxe-pager .vxe-pager--jump) {
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
  display: flex !important;
  align-items: center !important;
  gap: 4px !important;
}
:deep(.vxe-pager .vxe-pager--jump input) {
  width: 50px !important;
  text-align: center !important;
}

:deep(.ant-tag-green) {
  background: #f6ffed;
  border-color: #b7eb8f;
  color: #52c41a;
}
:deep(.ant-tag-blue) {
  background: #e6f4ff;
  border-color: #91caff;
  color: #1677ff;
}
:deep(.ant-tag-orange) {
  background: #fff7e6;
  border-color: #ffd591;
  color: #fa8c16;
}

/* 备注列：单行省略，hover 由 antd Tooltip 显示全量 */
.role-remark-cell {
  display: inline-block;
  max-width: 100%;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  vertical-align: middle;
}

/*
 * 工具栏批量按钮配色（与用户管理保持一致）
 *  - .btn-success       绿色实色（修改）
 *  - .btn-danger-outline 红色描边（删除）
 */
.btn-success {
  color: #52c41a;
  border-color: #b7eb8f;
  background: #f6ffed;
}
.btn-success:hover,
.btn-success:focus {
  color: #389e0d;
  border-color: #52c41a;
  background: #d9f7be;
}
.btn-success:disabled {
  color: rgba(0, 0, 0, 0.25);
  border-color: #d9d9d9;
  background: #f5f5f5;
}
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
.btn-danger-outline:disabled {
  color: rgba(0, 0, 0, 0.25);
  border-color: #d9d9d9;
  background: #f5f5f5;
}
</style>
