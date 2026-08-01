<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { RoleDto as SystemRole } from '#/api/vision-archive/system';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { Plus } from '@vben/icons';

import { Button, message, Modal } from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { roleApi } from '#/api/vision-archive/system';

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
    await roleApi.update(row.id, { status: newStatus } as any);
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

const [Grid, gridApi] = useVbenVxeGrid({
  formOptions: {
    schema: useGridFormSchema(),
    submitOnChange: true,
  },
  gridOptions: {
    columns: useColumns(onStatusChange),
    height: 'auto',
    keepSource: true,
    proxyConfig: {
      ajax: {
        query: async ({ page }, formValues) => {
          return await roleApi.getPagedList({
            page: page.currentPage,
            pageSize: page.pageSize,
            ...formValues,
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
          新增角色
        </Button>
      </template>
      <template #action="{ row }">
        <VbenTableAction
          :actions="[
            { text: '编辑', icon: 'lucide:edit', onClick: () => onEdit(row) },
          ]"
          :dropdown-actions="[
            {
              text: '删除',
              icon: 'lucide:trash-2',
              danger: true,
              popConfirm: { title: '确认删除此角色?', confirm: () => onDelete(row) },
            },
          ]"
          align="center"
        />
      </template>
    </Grid>
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
</style>
