<script lang="ts" setup>
/**
 * 组织管理 - 主页面（树表 + 级别标签）
 *
 * 关键设计：
 * 1. 数据源：用 orgApi.getTree() 拉整棵树，proxyConfig 直接返回（不走分页）
 * 2. 树形展示：treeConfig + 名称列 treeNode:true
 * 3. 级别可视化：组织级别（国家级/省级/市级/区县级/学校级）以彩色 Tag 挂在名称前，
 *    国家级红、省级橙、市级蓝、区县级青、学校级绿——一眼看出层级
 * 4. 搜索：客户端按名称 + 状态过滤（树形结构过滤要保留父链）
 * 5. 工具栏：新增 / 展开折叠全部
 */
import type { VxeTableGridOptions } from '#/adapter/vxe-table';
import type { OrgDto as SystemOrg } from '#/api/vision-archive/system';

import { nextTick, ref } from 'vue';

import { Page, useVbenDrawer } from '@vben/common-ui';
import { IconifyIcon, Plus } from '@vben/icons';

import { Button, message, Space, Tag } from 'ant-design-vue';

import { useVbenVxeGrid, VbenTableAction } from '#/adapter/vxe-table';
import { orgApi } from '#/api/vision-archive/system';

import {
  ensureLevelDictLoaded,
  getLevelColor,
  getLevelLabel,
  useColumns,
  useGridFormSchema,
} from './data';
import Form from './modules/form.vue';

const [FormDrawer, formDrawerApi] = useVbenDrawer({
  connectedComponent: Form,
  destroyOnClose: true,
});

function onEdit(row: SystemOrg) {
  formDrawerApi.setData(row).open();
}

/**
 * 在当前组织下新增子组织
 * 把 parentId 透传给 drawer，form.vue 会在提交时挂到 body 上
 */
function onAppend(row: SystemOrg) {
  formDrawerApi.setData({ parentId: row.id }).open();
}

async function onDelete(row: SystemOrg) {
  try {
    gridApi.setLoading(true);
    await orgApi.delete(row.id);
    message.success('删除成功');
    await gridApi.reload();
  } catch (err) {
    console.error('删除组织失败', err);
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

async function onStatusChange(newStatus: number, row: SystemOrg) {
  const text = newStatus === 1 ? '启用' : '停用';
  try {
    gridApi.setLoading(true);
    await orgApi.update(row.id, { status: newStatus } as any);
    message.success(`已${text}【${row.name}】`);
    return true;
  } catch {
    return false;
  } finally {
    gridApi.setLoading(false);
  }
}

/** 树表全部展开 / 折叠切换 */
const isAllExpanded = ref(true);
function toggleExpand() {
  const grid = (gridApi as any).grid;
  if (!grid) return;
  isAllExpanded.value = !isAllExpanded.value;
  grid.setAllTreeExpand(isAllExpanded.value);
}

/**
 * 搜索/重置后让树表保持展开。
 * 直接在 query 返回前用 nextTick 排个微任务：等 vxe-table 把新数据 setTableData、
 * 走完内部 reconcile 之后再调 setAllTreeExpand，避免在数据还没就位时调用导致展开失效。
 */
function expandAllOnNextTick() {
  nextTick(() => {
    const grid = (gridApi as any).grid;
    if (!grid) return;
    grid.setAllTreeExpand(true);
    isAllExpanded.value = true;
  });
}

/**
 * 客户端按名称/状态过滤组织树
 * 树形过滤的特点：子节点命中但父节点不命中时，父节点要保留（不然树就断了）
 */
function filterTree(
  tree: SystemOrg[],
  formValues: { name?: string; status?: number },
): SystemOrg[] {
  const name = formValues?.name?.trim()?.toLowerCase();
  const status = formValues?.status;
  const walk = (nodes: SystemOrg[]): SystemOrg[] => {
    const result: SystemOrg[] = [];
    for (const node of nodes) {
      const children = node.children?.length ? walk(node.children) : [];
      const nameMatch = !name || node.name.toLowerCase().includes(name);
      const statusMatch = status === undefined || node.status === status;
      if (nameMatch && statusMatch) {
        result.push({ ...node, children });
      } else if (children.length > 0) {
        // 父节点本身不命中但子节点命中，保留父节点并只挂命中的子节点
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
        // vben-form 的表单值由 packages/effects/plugins/src/vxe-table/extends.ts
        // 注入到 query 的「第二参数」里（第一参数里的 formValues 字段在 vxe-table 4.x 已废弃，
        // 写错位置会导致永远拿不到值，filter 永远走不过滤分支）
        query: async (_params, formValues) => {
          // 并行拉树和字典：ensureLevelDictLoaded 是单例 Promise，重复调用不会重复打接口
          const [tree] = await Promise.all([
            orgApi.getTree(),
            ensureLevelDictLoaded(),
          ]);
          const fv = (formValues || {}) as { name?: string; status?: number };
          if (!fv.name && fv.status === undefined) {
            expandAllOnNextTick();
            return tree;
          }
          const filtered = filterTree(tree, fv);
          expandAllOnNextTick();
          return filtered;
        },
      },
    },
    rowConfig: {
      keyField: 'id',
    },
    toolbarConfig: {
      // 不显示 VxeTable 自带的左侧标题和工具按钮，全部用自定义工具栏
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
  } as VxeTableGridOptions<SystemOrg>,
});
</script>
<template>
  <Page auto-content-height>
    <FormDrawer @success="onRefresh" />
    <Grid table-title="组织列表">
      <template #toolbar-tools>
        <Space>
          <Button type="primary" @click="onCreate">
            <Plus class="size-4" />
            新增组织
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

      <!-- 组织名称列：级别彩色 Tag + 名称 -->
      <template #name="{ row }">
        <div class="flex w-full items-center gap-2">
          <Tag
            v-if="row.level"
            :color="getLevelColor(row.level)"
            class="!m-0 shrink-0"
          >
            {{ getLevelLabel(row.level) }}
          </Tag>
          <span class="truncate">{{ row.name }}</span>
        </div>
      </template>

      <template #action="{ row }">
        <VbenTableAction
          :actions="[
            { text: '修改', icon: 'lucide:edit', onClick: () => onEdit(row) },
            { text: '新增', icon: 'lucide:plus', onClick: () => onAppend(row) },
            {
              text: '删除',
              icon: 'lucide:trash-2',
              danger: true,
              popConfirm: {
                title: '确认删除该组织?',
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
