<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { RoleDto as SystemRole } from '#/api/vision-archive/system';

import { computed, ref } from 'vue';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { Download, Plus } from '@vben/icons';

import {
  Button,
  Drawer,
  Form as AntForm,
  FormItem,
  Input,
  message,
  Modal,
  Popconfirm,
  Select,
  Tooltip,
} from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { DATA_SCOPE_OPTIONS, roleApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema } from './data';
import Form from './modules/form.vue';

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: Form,
  destroyOnClose: true,
});

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
    message.success('状态更新成功');
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

function onToolbarEdit() {
  const rows = getCheckedRows();
  if (rows.length === 0) {
    message.warning('请先选择一行');
    return;
  }
  if (rows.length > 1) {
    message.warning('一次只能修改一行，请勿多选');
    return;
  }
  onEdit(rows[0]);
}

async function onToolbarDelete() {
  const rows = getCheckedRows();
  if (rows.length === 0) {
    message.warning('请先选择要删除的角色');
    return;
  }
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
const dataScopeDrawer = ref<{
  open: boolean;
  row: SystemRole | null;
  dataScope: string;
}>({
  open: false,
  row: null,
  dataScope: 'ALL',
});

async function onAssignDataScope(row: SystemRole) {
  // 拉取角色当前的数据权限，回填到 Drawer
  let initial: string = row.dataScope ?? 'ALL';
  try {
    const cur = await roleApi.getDataPermission(row.id);
    initial = cur?.dataScope ?? initial;
  } catch {
    // 拉不到当前值就用 row.dataScope 兜底
  }
  dataScopeDrawer.value = { open: true, row, dataScope: initial };
}

async function onConfirmDataScope() {
  const m = dataScopeDrawer.value;
  if (!m.row) return;
  try {
    await roleApi.assignDataPermission(m.row.id, { dataScope: m.dataScope });
    message.success(
      `已为【${m.row.name}】分配数据权限：${
        DATA_SCOPE_OPTIONS.find((o) => o.value === m.dataScope)?.label ?? m.dataScope
      }`,
    );
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

// 内置系统角色（roleCode === 'admin'）不允许任何操作
const isSystemRole = (row: SystemRole) => row.roleCode === 'admin';

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
  gridOptions: {
    columns: [
      { type: 'checkbox', width: 50 },
      ...(useColumns(onStatusChange) as any[]),
    ],
    height: 'auto',
    keepSource: true,
    proxyConfig: {
      ajax: {
        query: async ({ page }, formValues = {}) => {
          // 把 RangePicker 的 [start, end] 拆成后端要的 startDate / endDate
          const { createdAtRange, ...rest } = formValues as Record<string, any>;
          const [startDate, endDate] = Array.isArray(createdAtRange) ? createdAtRange : [undefined, undefined];
          return await roleApi.getPagedList({
            page: page.currentPage,
            pageSize: page.pageSize,
            ...rest,
            startDate,
            endDate,
          });
        },
      },
    },
    rowConfig: {
      keyField: 'id',
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
        <Button type="primary" @click="onCreate">
          <Plus class="size-5" />
          新增
        </Button>
        <Button
          type="primary"
          ghost
          @click="onToolbarEdit"
        >
          <Edit class="size-4" />
          修改
        </Button>
        <Popconfirm
          :title="`确定要删除选中的角色吗?`"
          @confirm="onToolbarDelete"
        >
          <Button danger ghost>
            <Trash2 class="size-4" />
            删除
          </Button>
        </Popconfirm>
        <Button @click="onToolbarExport">
          <Download class="size-4" />
          导出
        </Button>
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
            v-model:value="dataScopeDrawer.dataScope"
            :options="DATA_SCOPE_OPTIONS"
            placeholder="请选择数据权限"
            class="w-full"
          />
        </FormItem>
      </AntForm>
      <template #footer>
        <Button @click="dataScopeDrawer.open = false">取消</Button>
        <Button type="primary" @click="onConfirmDataScope">确定</Button>
      </template>
    </Drawer>
  </Page>
</template>

<style scoped>
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
</style>
