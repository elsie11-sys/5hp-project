<script lang="ts" setup>
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { useVbenForm } from '#/adapter/form';
import { orgApi } from '#/api/vision-archive/system';

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
        await orgApi.update(id.value, values as any);
      } else {
        await orgApi.create(values as any);
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
      if (data) {
        id.value = data.id;
        await nextTick();
        formApi.setValues({
          name: data.name,
          code: data.code,
          level: data.level,
          sort: data.sort,
          status: data.status,
        });
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '编辑组织' : '新增组织'));
</script>
<template>
  <Drawer :title="getTitle">
    <Form />
  </Drawer>
</template>
