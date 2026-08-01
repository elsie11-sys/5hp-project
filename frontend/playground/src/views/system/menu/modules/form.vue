<script lang="ts" setup>
import type { VbenFormSchema } from '#/adapter/form';

import { computed, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { useVbenForm } from '#/adapter/form';
import {
  createMenu,
  getAllMenus,
  SystemMenuApi,
  updateMenu,
} from '#/api/system/menu';

const emit = defineEmits<{
  success: [];
}>();

const formData = ref<SystemMenuApi.SystemMenu>();

const menuTypeOptions = SystemMenuApi.MenuTypes.map((item) => ({
  label: item.label,
  value: item.value,
}));

const menuStatusOptions = SystemMenuApi.MenuStatus.map((item) => ({
  label: item.label,
  value: item.value,
}));

const YesNoOptions = [
  { label: '是', value: 1 },
  { label: '否', value: 0 },
];

const schema: VbenFormSchema[] = [
  {
    component: 'Select',
    componentProps: {
      allowClear: true,
      placeholder: '请选择上级菜单',
      filterOption: (input: string, option: { label: string }) => {
        return option.label.toLowerCase().includes(input.toLowerCase());
      },
      loadOptions: async () => {
        const menus = await getAllMenus();
        return menus.map((m) => ({
          label: m.name,
          value: m.id,
        }));
      },
      showSearch: true,
    },
    fieldName: 'parentId',
    label: '上级菜单',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: menuTypeOptions,
      optionType: 'button',
    },
    defaultValue: 1,
    fieldName: 'type',
    label: '菜单类型',
    rules: 'required',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '点击选择图标',
    },
    fieldName: 'icon',
    label: '菜单图标',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '请输入菜单名称',
    },
    fieldName: 'name',
    label: '菜单名称',
    rules: 'required',
  },
  {
    component: 'InputNumber',
    componentProps: {
      placeholder: '请输入显示排序',
      min: 0,
      max: 999,
      class: 'w-full',
    },
    defaultValue: 0,
    fieldName: 'sort',
    label: '显示排序',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: YesNoOptions,
      optionType: 'button',
    },
    defaultValue: 0,
    fieldName: 'isExternal',
    label: '是否外链',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '请输入路由地址',
    },
    fieldName: 'path',
    label: '路由地址',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '请输入组件路径',
    },
    fieldName: 'component',
    label: '组件路径',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '请输入权限字符',
    },
    fieldName: 'permission',
    label: '权限字符',
  },
  {
    component: 'Input',
    componentProps: {
      placeholder: '请输入路由参数',
    },
    fieldName: 'routeParams',
    label: '路由参数',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '缓存', value: 1 },
        { label: '不缓存', value: 0 },
      ],
      optionType: 'button',
    },
    defaultValue: 1,
    fieldName: 'isKeepAlive',
    label: '是否缓存',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: [
        { label: '显示', value: 1 },
        { label: '隐藏', value: 0 },
      ],
      optionType: 'button',
    },
    defaultValue: 1,
    fieldName: 'isVisible',
    label: '显示状态',
  },
  {
    component: 'RadioGroup',
    componentProps: {
      buttonStyle: 'solid',
      options: menuStatusOptions,
      optionType: 'button',
    },
    defaultValue: 1,
    fieldName: 'status',
    label: '菜单状态',
    rules: 'required',
  },
];

const [Form, formApi] = useVbenForm({
  commonConfig: {
    colon: true,
    formItemClass: 'col-span-2 md:col-span-1',
  },
  schema,
  showDefaultActions: false,
  wrapperClass: 'grid-cols-2 gap-x-4',
});

const [Drawer, drawerApi] = useVbenDrawer({
  onConfirm: onSubmit,
  onOpenChange(isOpen) {
    if (isOpen) {
      const data = drawerApi.getData<SystemMenuApi.SystemMenu>();
      if (data) {
        formData.value = data;
        formApi.setValues(formData.value);
      } else {
        formApi.resetForm();
      }
    }
  },
});

async function onSubmit() {
  const { valid } = await formApi.validate();
  if (valid) {
    drawerApi.lock();
    const data =
      await formApi.getValues<
        Omit<SystemMenuApi.SystemMenu, 'id' | 'children'>
      >();
    try {
      await (formData.value?.id
        ? updateMenu(formData.value.id as number, data)
        : createMenu(data));
      drawerApi.close();
      emit('success');
    } finally {
      drawerApi.unlock();
    }
  }
}

const getDrawerTitle = computed(() =>
  formData.value?.id ? '修改菜单' : '添加菜单',
);
</script>

<template>
  <Drawer class="w-full max-w-200" :title="getDrawerTitle">
    <Form class="mx-4" :layout="'horizontal'" />
  </Drawer>
</template>
