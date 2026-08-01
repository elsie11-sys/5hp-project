<script lang="ts" setup>
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { useVbenForm } from '#/adapter/form';
import { menuApi } from '#/api/vision-archive/system';

import { useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  schema: useFormSchema(),
  showDefaultActions: false,
});

const id = ref<number | string>();
const [Drawer, drawerApi] = useVbenDrawer({
  async onConfirm() {
    const { valid } = await formApi.validate();
    if (!valid) return;
    const values = await formApi.getValues();
    drawerApi.lock();
    try {
      if (id.value) {
        await menuApi.update(id.value, values as any);
      } else {
        await menuApi.create(values as any);
      }
      emits('success');
      drawerApi.close();
    } catch {
      drawerApi.unlock();
    }
  },
  async onOpenChange(isOpen) {
    if (isOpen) {
      const data = drawerApi.getData<any>();
      formApi.resetForm();
      id.value = undefined;
      await nextTick();
      if (data?.id) {
        // 编辑
        id.value = data.id;
        formApi.setValues({
          parentId: data.parentId,
          type: data.type,
          icon: data.icon,
          name: data.name,
          sort: data.sort,
          path: data.path,
          component: data.component,
          permission: data.permission,
          status: data.status,
        });
      } else if (data?.parentId) {
        // 新增子菜单：预置上级菜单
        formApi.setValues({
          parentId: data.parentId,
          type: 2,
          sort: 0,
          status: 1,
        });
      } else {
        // 新增根菜单
        formApi.setValues({
          type: 1,
          sort: 0,
          status: 1,
        });
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '修改菜单' : '新增菜单'));
</script>
<template>
  <Drawer :title="getTitle">
    <Form />
  </Drawer>
</template>
