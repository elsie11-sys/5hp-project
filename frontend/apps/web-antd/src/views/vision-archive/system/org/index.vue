<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { OrgDto as SystemOrg } from '#/api/vision-archive/system';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { Plus } from '@vben/icons';

import { Button, message, Modal } from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { orgApi } from '#/api/vision-archive/system';

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

async function onStatusChange(newStatus: number, row: SystemOrg) {
  const text = newStatus === 1 ? '正常' : '停用';
  try {
    await confirm(`确定要${text}组织【${row.name}】吗？`, '状态切换');
    await orgApi.update(row.id, { status: newStatus } as any);
    message.success('状态更新成功');
    return true;
  } catch {
    return false;
  }
}

function onEdit(row: SystemOrg) {
  formDrawerApi.setData(row).open();
}

async function onDelete(row: SystemOrg) {
  await confirm(`确定要删除组织【${row.name}】吗？`, '删除确认');
  await orgApi.delete(row.id);
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
          return await orgApi.getPagedList({
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
  } as VxeTableGridOptions<SystemOrg>,
});
</script>
<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <Grid table-title="组织列表">
      <template #toolbar-tools>
        <Button type="primary" @click="onCreate">
          <Plus class="size-5" />
          新增组织
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
              popConfirm: { title: '确认删除此组织?', confirm: () => onDelete(row) },
            },
          ]"
          align="center"
        />
      </template>
    </Grid>
  </Page>
</template>
