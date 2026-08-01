<script lang="ts" setup>
import type { MenuDto } from '#/api/vision-archive/system';

import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { Tree as ATree, Spin, message } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { menuApi, roleApi } from '#/api/vision-archive/system';

import { useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  schema: useFormSchema(),
  showDefaultActions: false,
});

const id = ref<number | string>();
const menuTree = ref<any[]>([]);
const checkedKeys = ref<Array<number | string>>([]);
const loadingMenu = ref(false);

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
}

async function handleConfirm() {
  const { valid } = await formApi.validate();
  if (!valid) return;
  const values = await formApi.getValues();
  drawerApi.lock();
  try {
    if (id.value) {
      await roleApi.update(id.value, {
        name: values.name,
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
      if (data) {
        id.value = data.id;
        await nextTick();
        formApi.setValues({
          name: data.name,
          code: data.code,
          level: data.level,
          status: data.status,
          remark: data.remark,
        });
        await loadMenuTree();
        await loadCheckedKeys(data.id);
      } else {
        await loadMenuTree();
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '编辑角色' : '新增角色'));

const defaultExpandedKeys = computed(() => menuTree.value.map((n) => n.key));
</script>

<template>
  <Drawer :title="getTitle">
    <Form />
    <div class="mt-4 border-t pt-3">
      <div class="mb-2 text-sm font-medium text-foreground/80">授权（菜单与按钮权限）</div>
      <Spin :spinning="loadingMenu" tip="加载菜单...">
        <ATree
          checkable
          :tree-data="menuTree"
          :checked-keys="checkedKeys"
          :default-expanded-keys="defaultExpandedKeys"
          @check="onCheck"
        />
      </Spin>
    </div>
  </Drawer>
</template>
