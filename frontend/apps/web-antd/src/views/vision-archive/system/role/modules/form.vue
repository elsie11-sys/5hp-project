<script lang="ts" setup>
/**
 * 角色管理 - 表单弹窗
 *
 * 对齐截图字段布局与交互：
 *   - 角色名称 *  ｜ 权限字符 *(带 ? 提示)
 *   - 等级 *(步进器，1~5，越小等级越高)
 *   - 状态（正常 / 停用，单选）
 *   - 菜单权限（展开/折叠、全选/全不选、父子联动 + 菜单树）
 *   - 备注（多行）
 *
 * 标题：添加角色 / 编辑角色
 */
import type { MenuDto } from '#/api/vision-archive/system';

import { computed, nextTick, ref, watch } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { Checkbox, Spin, Tree as ATree, message } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { menuApi, roleApi } from '#/api/vision-archive/system';

import { useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  commonConfig: {
    labelWidth: 90,
  },
  schema: useFormSchema(),
  showDefaultActions: false,
  // 单列布局，与截图一致
  wrapperClass: 'grid-cols-1',
});

const id = ref<number | string>();
const menuTree = ref<any[]>([]);
const checkedKeys = ref<Array<number | string>>([]);
const expandedKeys = ref<Array<number | string>>([]);
const loadingMenu = ref(false);

// 菜单权限工具栏状态
const expandAll = ref(true);
const selectAll = ref(false);
const checkStrictly = ref(false);

async function loadMenuTree() {
  if (menuTree.value.length > 0) return;
  loadingMenu.value = true;
  try {
    const tree = await menuApi.getTree();
    menuTree.value = normalizeTree(tree);
  } catch {
    menuTree.value = [];
  } finally {
    loadingMenu.value = false;
  }
}

function normalizeTree(list: MenuDto[] = []): any[] {
  return list.map((item) => ({
    key: item.id,
    title: item.name,
    type: item.type,
    children: item.children?.length ? normalizeTree(item.children) : undefined,
  }));
}

async function loadCheckedKeys(roleId: number | string) {
  try {
    const ids = await roleApi.getMenuPermission(roleId);
    checkedKeys.value = ids ?? [];
  } catch {
    checkedKeys.value = [];
  }
}

function onCheck(keys: any) {
  if (Array.isArray(keys)) {
    checkedKeys.value = keys;
  } else if (keys && Array.isArray((keys as any).checked)) {
    checkedKeys.value = (keys as any).checked;
  }
  // 同步"全选/全不选"复选框的勾选状态
  selectAll.value = isAllChecked();
}

// 收集所有 key
function collectAllKeys(nodes: any[] = []): Array<number | string> {
  const keys: Array<number | string> = [];
  const walk = (list: any[]) => {
    for (const n of list) {
      keys.push(n.key);
      if (n.children?.length) walk(n.children);
    }
  };
  walk(nodes);
  return keys;
}

const allKeys = computed(() => collectAllKeys(menuTree.value));

function isAllChecked() {
  if (allKeys.value.length === 0) return false;
  return allKeys.value.every((k) => checkedKeys.value.includes(k));
}

// 展开/折叠
function onToggleExpand(checked: boolean) {
  expandAll.value = checked;
  expandedKeys.value = checked ? [...allKeys.value] : [];
}

// 全选/全不选
function onToggleSelectAll(checked: boolean) {
  selectAll.value = checked;
  checkedKeys.value = checked ? [...allKeys.value] : [];
}

// 父子联动：antd Tree 通过 checkStrictly 控制；切换时保留已选项
watch(checkStrictly, () => {
  // 触发一次刷新：把当前 keys 重新写回去，强制 Tree 重新计算半选/全选状态
  const current = [...checkedKeys.value];
  checkedKeys.value = [];
  nextTick(() => {
    checkedKeys.value = current;
    selectAll.value = isAllChecked();
  });
});

async function handleConfirm() {
  const { valid } = await formApi.validate();
  if (!valid) return;
  const values = await formApi.getValues();
  drawerApi.lock();
  try {
    if (id.value) {
      await roleApi.update(id.value, {
        name: values.name,
        roleCode: values.roleCode,
        code: values.code,
        level: values.level,
        status: values.status,
        remark: values.remark,
      } as any);
      await roleApi.assignMenuPermission(id.value, checkedKeys.value as number[]);
      message.success('保存成功');
    } else {
      const created = await roleApi.create({
        name: values.name,
        roleCode: values.roleCode,
        code: values.code,
        level: values.level,
        status: values.status,
        remark: values.remark,
      } as any);
      if (created?.id && checkedKeys.value.length > 0) {
        await roleApi.assignMenuPermission(created.id, checkedKeys.value as number[]);
      }
      message.success('创建成功');
    }
    emits('success');
    drawerApi.close();
  } catch {
    drawerApi.unlock();
  }
}

const [Drawer, drawerApi] = useVbenDrawer({
  async onConfirm() {
    await handleConfirm();
  },
  async onOpenChange(isOpen) {
    if (isOpen) {
      const data = drawerApi.getData<any>();
      formApi.resetForm();
      id.value = undefined;
      checkedKeys.value = [];
      expandedKeys.value = [];
      expandAll.value = true;
      selectAll.value = false;
      checkStrictly.value = false;
      if (data && data.id) {
        id.value = data.id;
        await nextTick();
        formApi.setValues({
          name: data.name,
          roleCode: data.roleCode,
          code: data.code,
          level: data.level,
          status: data.status,
          remark: data.remark,
        });
        await loadMenuTree();
        await loadCheckedKeys(data.id);
        // 编辑场景：默认展开
        expandedKeys.value = [...allKeys.value];
        selectAll.value = isAllChecked();
      } else {
        await loadMenuTree();
        expandedKeys.value = [...allKeys.value];
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '编辑角色' : '添加角色'));
</script>

<template>
  <Drawer :title="getTitle">
    <Form />

    <!--
      菜单权限：label 在左、工具栏 + 树在右
      - 不用 form schema 渲染是为了能用复合控件（checkbox + tree）并让整体视觉跟截图一致
      - label 宽度 90px 与 commonConfig.labelWidth 对齐
    -->
    <div class="role-menu-permission">
      <div class="role-menu-permission__label">菜单权限</div>
      <div class="role-menu-permission__body">
        <div class="role-menu-permission__tools">
          <Checkbox
            v-model:checked="expandAll"
            @change="(e: any) => onToggleExpand(e.target.checked)"
          >
            展开/折叠
          </Checkbox>
          <Checkbox
            v-model:checked="selectAll"
            @change="(e: any) => onToggleSelectAll(e.target.checked)"
          >
            全选/全不选
          </Checkbox>
          <Checkbox v-model:checked="checkStrictly">
            <span class="text-green-600">父子联动</span>
          </Checkbox>
        </div>
        <Spin :spinning="loadingMenu" tip="加载菜单...">
          <div class="role-menu-permission__tree">
            <ATree
              v-model:checked-keys="checkedKeys"
              v-model:expanded-keys="expandedKeys"
              checkable
              :check-strictly="!checkStrictly"
              :tree-data="menuTree"
              @check="onCheck"
            />
          </div>
        </Spin>
      </div>
    </div>
  </Drawer>
</template>

<style scoped>
/*
 * 让 ant-design-vue InputNumber 的上/下箭头（.ant-input-number-handler-wrap）
 * 在 idle 状态下也常驻显示——默认是 opacity:0，只在 :hover / :focus 时才显示。
 * 用 :deep() 穿透 scoped 边界，影响 schema 里挂了 .always-show-controls 类的 InputNumber。
 */
:deep(.always-show-controls .ant-input-number-handler-wrap) {
  opacity: 1 !important;
}

:deep(.always-show-controls .ant-input-number-handler) {
  opacity: 1;
}

.role-menu-permission {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  margin-top: 16px;
}

.role-menu-permission__label {
  flex-shrink: 0;
  width: 90px;
  padding-right: 4px;
  text-align: right;
  font-size: 14px;
  font-weight: 500;
  color: hsl(var(--foreground));
  line-height: 32px;
}

.role-menu-permission__body {
  flex: 1;
  min-width: 0;
}

.role-menu-permission__tools {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 0 0 8px 0;
  font-size: 14px;
}

.role-menu-permission__tree {
  border: 1px solid hsl(var(--border));
  border-radius: 4px;
  padding: 8px 12px;
  background: hsl(var(--card));
  /* 菜单多时内部滚动，工具栏与外层 Drawer 高度保持稳定 */
  max-height: 300px;
  min-height: 200px;
  overflow-y: auto;
}

/*
 * 状态字段"正常"选中态绿色高亮（贴合截图）
 * 用 formItemClass="role-status-radio" 限定作用域，避免污染其他 Radio
 */
:deep(.role-status-radio .ant-radio-wrapper-checked) {
  color: #52c41a;
}

:deep(.role-status-radio .ant-radio-wrapper-checked .ant-radio-checked .ant-radio-inner) {
  background-color: #52c41a;
  border-color: #52c41a;
}
</style>
