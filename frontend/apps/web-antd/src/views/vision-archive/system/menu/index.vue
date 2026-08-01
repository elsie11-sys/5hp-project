<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { MenuDto as SystemMenu } from '#/api/vision-archive/system';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import { Button, message, Modal } from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { menuApi } from '#/api/vision-archive/system';

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

async function onStatusChange(newStatus: number, row: SystemMenu) {
  const text = newStatus === 1 ? '正常' : '停用';
  try {
    await confirm(`确定要${text}菜单【${row.name}】吗？`, '状态切换');
    await menuApi.update(row.id, { status: newStatus } as any);
    message.success('状态更新成功');
    return true;
  } catch {
    return false;
  }
}

function onEdit(row: SystemMenu) {
  formDrawerApi.setData(row).open();
}

function onAppend(row: SystemMenu) {
  formDrawerApi.setData({ parentId: row.id }).open();
}

async function onDelete(row: SystemMenu) {
  await menuApi.delete(row.id);
  message.success('删除成功');
  onRefresh();
}

function onRefresh() {
  gridApi.query();
}

function onCreate() {
  formDrawerApi.setData({}).open();
}

/** 客户端过滤菜单树（按名称/状态） */
function filterTree(tree: SystemMenu[], formValues: { name?: string; status?: number }): SystemMenu[] {
  const name = formValues?.name?.trim()?.toLowerCase();
  const status = formValues?.status;
  const walk = (nodes: SystemMenu[]): SystemMenu[] => {
    const result: SystemMenu[] = [];
    for (const node of nodes) {
      const children = node.children?.length ? walk(node.children) : [];
      const nameMatch = !name || node.name.toLowerCase().includes(name);
      const statusMatch = status === undefined || node.status === status;
      if (nameMatch && statusMatch) {
        result.push({ ...node, children });
      } else if (children.length > 0) {
        result.push({ ...node, children });
      }
    }
    return result;
  };
  return walk(tree);
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
    pagerConfig: {
      enabled: false,
    },
    proxyConfig: {
      ajax: {
        query: async ({ formValues }) => {
          const tree = await menuApi.getTree();
          const fv = formValues as { name?: string; status?: number } | undefined;
          if (!fv?.name && fv?.status === undefined) {
            return tree;
          }
          return filterTree(tree, fv);
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
    treeConfig: {
      parentField: 'parentId',
      rowField: 'id',
      transform: false,
    },
  } as VxeTableGridOptions<SystemMenu>,
});
</script>

<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <Grid table-title="菜单列表">
      <template #toolbar-tools>
        <Button type="primary" @click="onCreate">
          <Plus class="size-5" />
          新增菜单
        </Button>
      </template>
      <template #title="{ row }">
        <div class="flex w-full items-center gap-1">
          <div class="size-4 shrink-0">
            <IconifyIcon
              v-if="row.type === 3"
              icon="lucide:lock"
              class="size-full"
            />
            <IconifyIcon
              v-else-if="row.icon"
              :icon="row.icon"
              class="size-full"
            />
          </div>
          <span class="flex-auto">{{ row.name }}</span>
        </div>
      </template>
      <template #icon="{ row }">
        <IconifyIcon
          v-if="row.icon"
          :icon="row.icon"
          class="size-4"
        />
      </template>
      <template #action="{ row }">
        <VbenTableAction
          :actions="[
            { text: '修改', icon: 'lucide:pencil', onClick: () => onEdit(row) },
            { text: '新增', icon: 'lucide:plus', onClick: () => onAppend(row) },
          ]"
          :dropdown-actions="[
            {
              text: '删除',
              icon: 'lucide:trash-2',
              danger: true,
              popConfirm: { title: '确认删除此菜单?', confirm: () => onDelete(row) },
            },
          ]"
          align="center"
        />
      </template>
    </Grid>
  </Page>
</template>
