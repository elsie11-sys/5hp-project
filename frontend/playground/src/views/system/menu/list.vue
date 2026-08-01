<script lang="ts" setup>
import type {
  OnActionClickParams,
  VxeTableGridOptions,
} from '#/adapter/vxe-table';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import { Button, message } from 'antdv-next';

import { useVbenVxeGrid } from '#/adapter/vxe-table';
import {
  deleteMenu,
  getMenuList,
  SystemMenuApi,
} from '#/api/system/menu';

import { useColumns, useSearchFormSchema } from './data';
import Form from './modules/form.vue';

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: Form,
  destroyOnClose: true,
});

const [Grid, gridApi] = useVbenVxeGrid({
  formOptions: {
    schema: useSearchFormSchema(),
    submitOnChange: true,
  },
  gridOptions: {
    columns: useColumns(onActionClick),
    height: 'auto',
    keepSource: true,
    pagerConfig: {
      enabled: false,
    },
    proxyConfig: {
      ajax: {
        query: async ({ formValues }) => {
          return await getMenuList(formValues);
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
  } as VxeTableGridOptions<SystemMenuApi.SystemMenu>,
});

function onActionClick({
  code,
  row,
}: OnActionClickParams<SystemMenuApi.SystemMenu>) {
  switch (code) {
    case 'append': {
      onAppend(row);
      break;
    }
    case 'delete': {
      onDelete(row);
      break;
    }
    case 'edit': {
      onEdit(row);
      break;
    }
    default: {
      break;
    }
  }
}

function onRefresh() {
  gridApi.query();
}

function onEdit(row: SystemMenuApi.SystemMenu) {
  formDrawerApi.setData(row).open();
}

function onCreate() {
  formDrawerApi.setData({ type: 1, status: 1, sort: 0, isKeepAlive: 1, isVisible: 1, isExternal: 0 }).open();
}

function onAppend(row: SystemMenuApi.SystemMenu) {
  formDrawerApi.setData({ parentId: row.id, type: 2, status: 1, sort: 0, isKeepAlive: 1, isVisible: 1, isExternal: 0 }).open();
}

function onDelete(row: SystemMenuApi.SystemMenu) {
  const hideLoading = message.loading({
    content: `正在删除菜单 "${row.name}"...`,
    duration: 0,
    key: 'action_process_msg',
  });
  deleteMenu(row.id as number)
    .then(() => {
      message.success({
        content: `删除菜单 "${row.name}" 成功`,
        key: 'action_process_msg',
      });
      onRefresh();
    })
    .catch(() => {
      hideLoading();
    });
}
</script>

<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <Grid>
      <template #toolbar-tools>
        <Button type="primary" @click="onCreate">
          <Plus class="size-5" />
          新增菜单
        </Button>
      </template>
      <template #title="{ row }">
        <div class="flex w-full items-center gap-1">
          <div class="size-5 shrink-0">
            <IconifyIcon
              v-if="row.type === 3"
              icon="carbon:security"
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
          class="size-5"
        />
      </template>
    </Grid>
  </Page>
</template>

<style scoped>
:deep(.table-operations) {
  font-size: 14px;
}

:deep(.table-operations .ant-btn-link) {
  font-size: 14px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'PingFang SC',
    'Hiragino Sans GB', 'Microsoft YaHei', 'Helvetica Neue', Helvetica, Arial,
    sans-serif;
}

:deep(.table-operations .ant-btn-link .anticon) {
  font-size: 14px;
  margin-right: 2px;
}
</style>
