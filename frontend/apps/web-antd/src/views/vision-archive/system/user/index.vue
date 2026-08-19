<script lang="ts" setup>
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { BackendUserDto as SystemUser } from '#/api/vision-archive/system';

import { computed, onMounted, ref, watch } from 'vue';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import { Button, Dropdown, Input, Menu, message, Modal, Popconfirm, Tree as ATree, Upload } from 'ant-design-vue';

import { useVbenVxeGrid } from '#/adapter/vxe-table';
import { mapDtoToDisplay, orgApi, setOrgMapping, userApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema } from './data';
import UserForm from './modules/form.vue';

const orgList = ref<any[]>([]);
const orgSearchText = ref('');
const expandedKeys = ref<Array<number | string>>([]);
const selectedKeys = ref<Array<number | string>>([]);
const selectedOrgId = ref<number | string>('');
const selectedRowIds = ref<Array<number | string>>([]);

function getAllKeys(nodes: any[]): Array<number | string> {
  const keys: Array<number | string> = [];
  for (const node of nodes) {
    if (node.children && node.children.length) {
      keys.push(node.id);
      keys.push(...getAllKeys(node.children));
    }
  }
  return keys;
}

/** 递归收集选中节点及其所有子节点的 ID（用于按组织筛选用户） */
function getDescendantKeys(targetId: number | string): Array<number | string> {
  const collect = (node: any): Array<number | string> => {
    const keys: Array<number | string> = [node.id];
    if (node.children && node.children.length) {
      for (const child of node.children) {
        keys.push(...collect(child));
      }
    }
    return keys;
  };
  const find = (list: any[]): any => {
    for (const node of list) {
      if (node.id === targetId) return node;
      if (node.children && node.children.length) {
        const found = find(node.children);
        if (found) return found;
      }
    }
    return null;
  };
  const target = find(orgList.value);
  return target ? collect(target) : [];
}

/** 动态构建 orgId → orgName 映射（基于实际组织数据） */
const orgIdToName = computed<Record<string, string>>(() => {
  const map: Record<string, string> = {};
  const walk = (nodes: any[]) => {
    for (const node of nodes) {
      map[String(node.id)] = node.name;
      if (node.children?.length) walk(node.children);
    }
  };
  walk(orgList.value);
  return map;
});

const filteredOrgList = computed(() => {
  if (!orgSearchText.value) return orgList.value;
  const filter = (nodes: any[]): any[] =>
    nodes
      .map((node) => {
        const children = filter(node.children || []);
        const match =
          node.name?.includes(orgSearchText.value) ||
          node.code?.includes(orgSearchText.value);
        if (match || children.length) {
          return { ...node, children };
        }
        return null;
      })
      .filter(Boolean);
  return filter(orgList.value);
});

function expandAll() {
  expandedKeys.value = getAllKeys(filteredOrgList.value);
}

function collapseAll() {
  expandedKeys.value = [];
}

function onOrgMenuClick(info: any) {
  const key = info?.key || info;
  if (key === 'expand') expandAll();
  else if (key === 'collapse') collapseAll();
}

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: UserForm,
  destroyOnClose: true,
});

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

async function onStatusChange(newStatus: number, row: SystemUser) {
  const text = newStatus === 1 ? '启用' : '禁用';
  try {
    await confirm(`确定要${text}用户【${row.realName || row.username}】吗？`, '状态切换');
    const display = mapDtoToDisplay(row) as any;
    display.status = newStatus === 1 ? 'active' : 'inactive';
    await userApi.updateUser(row.id, display);
    message.success('状态更新成功');
    return true;
  } catch {
    return false;
  }
}

function onEdit(row: SystemUser) {
  formDrawerApi.setData(row).open();
}

function onDetail(row: SystemUser) {
  message.info(`用户详情：${row.username}（${row.realName}）`);
}

async function onResetPassword(row: SystemUser) {
  try {
    await confirm(`确定要将用户【${row.realName || row.username}】的密码重置为 123456 吗？`, '重置密码');
    await userApi.resetPassword(row.id);
    message.success('密码已重置为 123456');
  } catch {
    // 用户取消
  }
}

async function onDelete(row: SystemUser) {
  await userApi.deleteUser(row.id);
  message.success('删除成功');
  onRefresh();
}

async function onBatchDelete() {
  const ids = getSelectedIds();
  if (!ids.length) return;
  try {
    await confirm(`确定要删除选中的 ${ids.length} 个用户吗？`, '批量删除');
    const res = await userApi.batchDelete(ids);
    message.success(`成功删除 ${res.deletedCount} 个用户`);
    selectedRowIds.value = [];
    onRefresh();
  } catch {
    // 用户取消
  }
}

function onBatchEdit() {
  const ids = getSelectedIds();
  if (ids.length !== 1) return;
  const grid = gridApi.grid;
  const record = grid?.getCheckboxRecords?.().find((r: any) => r.id === ids[0])
    ?? grid?.getTableData?.().fullData?.find((r: any) => r.id === ids[0]);
  if (record) {
    formDrawerApi.setData(record).open();
  }
}

function onRefresh() {
  gridApi.query();
}

function onCreate() {
  formDrawerApi.setData({}).open();
}

async function handleImport(file: File) {
  const formData = new FormData();
  formData.append('file', file);
  try {
    const res = await userApi.importUsers(formData);
    message.success(`导入完成：成功 ${res.successCount} 条，失败 ${res.failedCount} 条`);
    if (res.errors?.length) {
      Modal.warning({
        title: '导入错误详情',
        content: (() => {
          const div = document.createElement('div');
          res.errors.slice(0, 10).forEach((e) => {
            const p = document.createElement('p');
            p.textContent = `第 ${e.row} 行：${e.message}`;
            div.appendChild(p);
          });
          if (res.errors.length > 10) {
            const p = document.createElement('p');
            p.textContent = `...还有 ${res.errors.length - 10} 条错误`;
            div.appendChild(p);
          }
          return div;
        }) as any,
      });
    }
    onRefresh();
  } catch {
    // 错误已由请求拦截器提示
  }
  return false; // 阻止 Upload 自动上传
}

function handleExport() {
  const params = new URLSearchParams();
  if (selectedOrgId.value) {
    params.append('orgIds', getDescendantKeys(selectedOrgId.value).join(','));
  }
  const qs = params.toString();
  // 生成精确到微秒的时间戳：toISOString 精确到毫秒，再用 performance.now() 的小数部分补 3 位微秒
  const now = new Date();
  const ms = now.getMilliseconds().toString().padStart(3, '0');
  const micro = Math.floor((performance.now() % 1) * 1000)
    .toString()
    .padStart(3, '0');
  const ts = `${now.getFullYear()}${String(now.getMonth() + 1).padStart(2, '0')}${String(now.getDate()).padStart(2, '0')}_${String(now.getHours()).padStart(2, '0')}${String(now.getMinutes()).padStart(2, '0')}${String(now.getSeconds()).padStart(2, '0')}_${ms}${micro}`;
  const a = document.createElement('a');
  a.href = `/api/user/export${qs ? '?' + qs : ''}`;
  a.download = `用户导出_${ts}.xlsx`;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  message.success('导出已开始');
}

const [Grid, gridApi] = useVbenVxeGrid({
  formOptions: {
    schema: useGridFormSchema(),
    submitOnChange: true,
  },
  gridEvents: {
    checkboxChange() {
      selectedRowIds.value = getSelectedIds();
    },
    checkboxAll() {
      selectedRowIds.value = getSelectedIds();
    },
  },
  gridOptions: {
    columns: [
      { type: 'checkbox', width: 50 },
      ...(useColumns(onStatusChange) as any[]),
    ],
    height: 'auto',
    keepSource: true,
    showOverflow: true,
    columnConfig: {
      resizable: true,
    },
    proxyConfig: {
      ajax: {
        query: async ({ page }, formValues) => {
          const fv = (formValues || {}) as {
            account?: string;
            phone?: string;
            status?: number;
          };
          const res = await userApi.getUserList({
            page: page.currentPage,
            pageSize: page.pageSize,
            keyword: fv.account || '',
            status: fv.status,
            orgIds: selectedOrgId.value
              ? getDescendantKeys(selectedOrgId.value).join(',')
              : undefined,
          } as any);
          return {
            items: res.items.map((item) => {
              const display = mapDtoToDisplay(item);
              const orgName =
                orgIdToName.value[String(item.orgId)] ?? display.org;
              return {
                ...item,
                roleName: display.role,
                orgName,
              };
            }),
            total: res.total,
          };
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
  } as VxeTableGridOptions<SystemUser>,
});

function getSelectedIds(): Array<number | string> {
  const grid = gridApi.grid;
  if (!grid) return [];
  const records = grid.getCheckboxRecords?.() || [];
  return records.map((r: any) => r.id);
}

async function loadOrgList() {
  try {
    const res = await orgApi.getTree();
    orgList.value = res as any;
    setOrgMapping(orgList.value);
    expandedKeys.value = getAllKeys(orgList.value);
  } catch {
    orgList.value = [];
  }
}

function onTreeExpand(keys: Array<number | string>) {
  expandedKeys.value = keys;
}

function onTreeSelect(keys: Array<number | string>) {
  selectedKeys.value = keys;
  selectedOrgId.value = keys[0] ?? '';
  gridApi.query();
}

watch(orgSearchText, () => {
  if (orgSearchText.value) {
    expandedKeys.value = getAllKeys(filteredOrgList.value);
  } else {
    expandedKeys.value = getAllKeys(orgList.value);
  }
});

onMounted(async () => {
  await loadOrgList();
});
</script>
<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <div class="flex h-full gap-2">
      <!-- 左侧组织列表 -->
      <div class="w-56 shrink-0 rounded-md bg-card">
        <div class="flex items-center justify-between border-b border-border px-3 py-2">
          <span class="text-sm font-medium text-foreground">组织列表</span>
          <Dropdown trigger="click">
            <Button type="text" size="small" class="px-1" @click.prevent>
              <IconifyIcon icon="lucide:more-vertical" class="size-4 text-foreground/60" />
            </Button>
            <template #overlay>
              <Menu @click="onOrgMenuClick">
                <Menu.Item key="expand">展开全部</Menu.Item>
                <Menu.Item key="collapse">折叠全部</Menu.Item>
              </Menu>
            </template>
          </Dropdown>
        </div>
        <div class="px-3 py-2">
          <Input
            v-model:value="orgSearchText"
            placeholder="搜索组织"
            allow-clear
            size="small"
          >
            <template #prefix>
              <IconifyIcon icon="lucide:search" class="size-3.5 text-foreground/40" />
            </template>
          </Input>
        </div>
        <div class="org-tree-wrap overflow-auto px-1 pb-2">
          <ATree
            :expanded-keys="expandedKeys"
            :selected-keys="selectedKeys"
            :tree-data="filteredOrgList"
            :field-names="{ children: 'children', title: 'name', key: 'id' }"
            block-node
            @expand="onTreeExpand"
            @select="onTreeSelect"
          />
        </div>
      </div>

      <!-- 右侧表格区 -->
      <div class="min-w-0 flex-1 overflow-hidden">
        <Grid table-title="账号列表">
          <template #toolbar-tools>
            <div class="flex items-center gap-2">
              <Button type="primary" @click="onCreate">
                <Plus class="mr-1 size-4" />
                新增
              </Button>
              <Button
                class="btn-success"
                :disabled="!hasSingleSelection"
                @click="onBatchEdit"
              >
                <IconifyIcon icon="lucide:pencil" class="mr-1 size-4" />
                修改
              </Button>
              <Button
                class="btn-danger-outline"
                :disabled="!hasSelection"
                @click="onBatchDelete"
              >
                <IconifyIcon icon="lucide:trash-2" class="mr-1 size-4" />
                删除
              </Button>
              <Upload
                :before-upload="handleImport"
                accept=".xlsx,.xls,.csv"
                :show-upload-list="false"
              >
                <Button>
                  <IconifyIcon icon="lucide:upload" class="mr-1 size-4" />
                  导入
                </Button>
              </Upload>
              <Button @click="handleExport">
                <IconifyIcon icon="lucide:download" class="mr-1 size-4" />
                导出
              </Button>
            </div>
          </template>
          <template #action="{ row }">
            <div class="flex items-center justify-center gap-1">
              <Button
                size="small"
                type="link"
                title="修改"
                @click="onEdit(row)"
              >
                <IconifyIcon icon="lucide:pencil" class="size-4" />
              </Button>
              <Button
                size="small"
                type="link"
                title="详情"
                @click="onDetail(row)"
              >
                <IconifyIcon icon="lucide:eye" class="size-4" />
              </Button>
              <Button
                size="small"
                type="link"
                danger
                title="重置密码"
                @click="onResetPassword(row)"
              >
                <IconifyIcon icon="lucide:key-round" class="size-4" />
              </Button>
              <Popconfirm
                title="确认删除此用户?"
                @confirm="onDelete(row)"
              >
                <Button
                  size="small"
                  type="link"
                  danger
                  title="删除"
                >
                  <IconifyIcon icon="lucide:trash-2" class="size-4" />
                </Button>
              </Popconfirm>
            </div>
          </template>
        </Grid>
      </div>
    </div>
  </Page>
</template>

<style scoped>
.org-tree-wrap {
  max-height: calc(100vh - 280px);
}
.org-tree-wrap :deep(.ant-tree) {
  background: transparent;
  font-size: 13px;
}
.org-tree-wrap :deep(.ant-tree-node-content-wrapper) {
  border-radius: 4px;
  padding: 2px 6px !important;
  transition: background-color 0.15s ease;
}
.org-tree-wrap :deep(.ant-tree-node-content-wrapper:hover) {
  background-color: hsl(var(--muted) / 0.5);
}
.org-tree-wrap :deep(.ant-tree-node-selected .ant-tree-node-content-wrapper) {
  background-color: hsl(var(--primary) / 0.12) !important;
  color: hsl(var(--primary));
  font-weight: 500;
}
.org-tree-wrap :deep(.ant-tree-switcher) {
  cursor: pointer;
}
.org-tree-wrap :deep(.ant-tree-switcher-close),
.org-tree-wrap :deep(.ant-tree-switcher-open) {
  display: flex;
  align-items: center;
  justify-content: center;
}
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

/* ========== 分页样式（antd 默认浅色风格 + 靠右对齐） ========== */
.grid-container :deep(.vxe-pager) {
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

.grid-container :deep(.vxe-pager .vxe-pager--total) {
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
  margin-right: 0 !important;
}

.grid-container :deep(.vxe-pager .vxe-pager--btn) {
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
.grid-container :deep(.vxe-pager .vxe-pager--btn:hover) {
  color: #52c41a !important;
  background: #f5f5f5 !important;
  border-color: transparent !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--btn.is--active) {
  background: #52c41a !important;
  border-color: #52c41a !important;
  color: #fff !important;
  font-weight: 500 !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--btn.is--disabled) {
  color: rgba(0, 0, 0, 0.25) !important;
  cursor: not-allowed !important;
  background: #f5f5f5 !important;
  border-color: transparent !important;
}

.grid-container :deep(.vxe-pager .vxe-pager--sizes) {
  margin: 0 !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--sizes select),
.grid-container :deep(.vxe-pager .vxe-pager--jump input) {
  background: #fff !important;
  border: 1px solid #d9d9d9 !important;
  color: rgba(0, 0, 0, 0.85) !important;
  border-radius: 4px !important;
  font-size: 13px !important;
  height: 30px !important;
  padding: 0 8px !important;
  transition: all 0.2s ease !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--sizes select:focus),
.grid-container :deep(.vxe-pager .vxe-pager--jump input:focus) {
  outline: none !important;
  border-color: #52c41a !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--jump) {
  color: rgba(0, 0, 0, 0.65) !important;
  font-size: 13px !important;
  display: flex !important;
  align-items: center !important;
  gap: 4px !important;
}
.grid-container :deep(.vxe-pager .vxe-pager--jump input) {
  width: 50px !important;
  text-align: center !important;
}
</style>
