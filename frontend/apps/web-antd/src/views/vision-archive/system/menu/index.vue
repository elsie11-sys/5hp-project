<script lang="ts" setup>
/**
 * 菜单管理 - 主页面
 *
 * 对齐 vben-v2 src/views/demo/system/menu/index.vue 的菜单管理交互：
 *   - 顶部搜索条：菜单名称 + 状态（placeholder 改为"菜单状态"）
 *   - 工具栏：新增 + 展开/折叠 全部
 *   - 树形菜单表格：菜单名称 / 图标 / 排序 / 权限标识 / 组件路径 / 状态 / 创建时间 / 操作
 *   - 行操作（全部内联）：修改 / 新增 / 删除
 */
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { MenuDto as SystemMenu } from '#/api/vision-archive/system';

import { ref } from 'vue';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import { Button, message, Space } from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { menuApi } from '#/api/vision-archive/system';

import { useColumns, useGridFormSchema } from './data';
import Form from './modules/form.vue';

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: Form,
  destroyOnClose: true,
  // 弹窗宽度：880px 让 2 列布局的字段（菜单名称/上级菜单、是否外链/路由地址 等）有更舒展的呼吸空间
  // vben v5 的 Drawer 用 Tailwind 类控制宽度，没有 width 属性——用 class 覆盖默认的 w-130
  class: '!w-[880px]',
});

function onEdit(row: SystemMenu) {
  formDrawerApi.setData(row).open();
}

function onAppend(row: SystemMenu) {
  // 把 parent 的 type 也传进去，让 form 弹窗做"智能默认"：
  //   目录(type=1) 下新增 → 默认 菜单(type=2)
  //   菜单(type=2) 下新增 → 默认 按钮(type=3)
  formDrawerApi
    .setData({ parentId: row.id, parentType: row.type })
    .open();
}

/**
 * 删除菜单：popConfirm 已经是气泡确认框了，这里不要再开 Modal.confirm，
 * 否则会双层弹窗，而且 popConfirm.confirm() 不 await 这个 Promise，
 * 内部 reject(new Error('取消')) 会变成 unhandled promise rejection → 控制台报错。
 */
async function onDelete(row: SystemMenu) {
  try {
    gridApi.setLoading(true);
    await menuApi.delete(row.id);
    message.success('删除成功');
    // 树表用 reload 强制清掉旧 source（keepSource:true 会让 query 不更新已删除行），
    // 然后 await 确保刷新完成再关 loading，避免用户看到 loading 闪一下就消失但表格没更新
    await gridApi.reload();
  } catch (err) {
    // requestClient 拦截器已经弹了 message.error，这里只记日志避免 unhandled rejection
    console.error('删除菜单失败', err);
  } finally {
    gridApi.setLoading(false);
  }
}

async function onRefresh() {
  await gridApi.reload();
}

function onCreate() {
  formDrawerApi.setData({}).open();
}

/**
 * 图标名归一化：没有 "collection:" 前缀的，统一补 ant-design:
 * （菜单表里的 icon 字段历史数据是 "settings"、"user" 这种短名，iconify 找不到）
 */
function normalizeIcon(name: string): string {
  if (!name) return '';
  return name.includes(':') ? name : `ant-design:${name}`;
}

/** 树表全部展开 / 折叠切换 */
const isAllExpanded = ref(true);
function toggleExpand() {
  const grid = (gridApi as any).grid;
  if (!grid) return;
  isAllExpanded.value = !isAllExpanded.value;
  // vxe-table: setAllTreeExpand(true) 全展开 / false 全折叠
  grid.setAllTreeExpand(isAllExpanded.value);
}

/** 客户端过滤菜单树（按名称/状态） */
function filterTree(
  tree: SystemMenu[],
  formValues: { name?: string; status?: number },
): SystemMenu[] {
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
    columns: useColumns(),
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
      // 不显示 VxeTable 自带的左侧标题，改为自定义工具栏
      custom: false,
      export: false,
      refresh: false,
      search: false,
      zoom: false,
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
    <Grid>
      <!-- 工具栏：左侧 新增 / 展开折叠 -->
      <template #toolbar-tools>
        <Space>
          <Button type="primary" @click="onCreate">
            <Plus class="size-4" />
            新增
          </Button>
          <Button @click="toggleExpand">
            <IconifyIcon
              :icon="
                isAllExpanded
                  ? 'lucide:chevrons-up-down'
                  : 'lucide:chevrons-down-up'
              "
              class="size-4"
            />
            {{ isAllExpanded ? '折叠' : '展开' }}
          </Button>
        </Space>
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
              :icon="normalizeIcon(row.icon)"
              class="size-full"
            />
          </div>
          <span class="flex-auto">{{ row.name }}</span>
        </div>
      </template>
      <template #icon="{ row }">
        <IconifyIcon
          v-if="row.icon"
          :icon="normalizeIcon(row.icon)"
          class="size-5"
        />
      </template>

      <!-- 操作列：修改 / 新增 / 删除 全部内联（参考截图） -->
      <template #action="{ row }">
        <VbenTableAction
          :actions="[
            {
              text: '修改',
              icon: 'lucide:pencil',
              onClick: () => onEdit(row),
            },
            {
              text: '新增',
              icon: 'lucide:plus',
              onClick: () => onAppend(row),
            },
            {
              text: '删除',
              icon: 'lucide:trash-2',
              danger: true,
              popConfirm: {
                title: '是否确认删除',
                confirm: () => onDelete(row),
              },
            },
          ]"
          align="center"
        />
      </template>
    </Grid>
  </Page>
</template>
