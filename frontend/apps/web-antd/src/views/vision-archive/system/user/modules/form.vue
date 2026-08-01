<script lang="ts" setup>
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { message } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { mapDtoToDisplay, userApi } from '#/api/vision-archive/system';

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
    // 新建时密码必填
    if (!id.value && !values.password) {
      message.warning('请输入密码');
      return;
    }
    // 编辑时密码留空则不传
    if (id.value && !values.password) {
      delete values.password;
    }
    drawerApi.lock();
    try {
      if (id.value) {
        await userApi.updateUser(id.value, values as any);
      } else {
        await userApi.createUser(values as any);
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
        const display = mapDtoToDisplay(data);
        formApi.setValues({
          username: display.username,
          name: display.name,
          gender: display.gender,
          role: display.role,
          org: display.org,
          phone: display.phone,
          email: display.email,
          status: display.status,
          password: '',
        });
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '编辑用户' : '新增用户'));
</script>
<template>
  <Drawer :title="getTitle">
    <Form />
  </Drawer>
</template>
