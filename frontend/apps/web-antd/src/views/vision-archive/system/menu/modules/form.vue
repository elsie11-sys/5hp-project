<script lang="ts" setup>
/**
 * 菜单管理 - 表单弹窗
 *
 * 对齐截图字段布局与交互：
 *   - 上级菜单 / 菜单类型 / 菜单图标 各占一行（span=24）
 *   - 菜单名称 + 显示排序 / 是否外链 + 路由地址 / 组件路径 + 权限字符 /
 *     路由参数 + 是否缓存 / 显示状态 + 菜单状态（每行两个）
 *   - 菜单类型 1=目录 2=菜单 3=按钮
 *   - 标题：修改菜单 / 新增菜单
 *
 * 字段命名与后端 C# 实体对齐（PascalCase → ASP.NET Core 默认 camelCase JSON）：
 *   SysMenu.IsExternal   → isExternal
 *   SysMenu.IsKeepAlive  → isKeepAlive
 *   SysMenu.IsVisible    → isVisible
 *   SysMenu.RouteParams  → routeParams
 */
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { useVbenForm } from '#/adapter/form';
import { menuApi, type MenuForm } from '#/api/vision-archive/system';

import { useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  schema: useFormSchema(),
  showDefaultActions: false,
  // 整体布局：小屏 1 列、md 及以上 2 列；全宽字段在 schema 上用 md:cols-span-full 覆盖
  wrapperClass: 'grid-cols-1 md:grid-cols-2',
  // 通用后备配置：标签列固定 100px，让 880px 弹框里两列布局更整齐
  commonConfig: {
    labelWidth: 100,
  },
});

const id = ref<number | string>();

const [Drawer, drawerApi] = useVbenDrawer({
  async onConfirm() {
    const { valid } = await formApi.validate();
    if (!valid) return;
    const values = (await formApi.getValues()) as Partial<MenuForm>;
    drawerApi.lock();
    try {
      // 强类型提交：表单字段名与后端 MenuForm 字段已对齐（camelCase）
      // 后端 SysMenuController 接收 JSON，反序列化到 MenuForm，MenuService 写入 DB
      // 用 ?? 给关键字段兜底，避免 { ...values } 把 undefined 的字段覆盖掉默认值
      const payload: MenuForm = {
        name: values.name ?? '',
        type: (values.type ?? 1) as 1 | 2 | 3,
        sort: values.sort ?? 0,
        status: (values.status ?? 1) as 0 | 1,
        isExternal: (values.isExternal ?? 0) as 0 | 1,
        isKeepAlive: (values.isKeepAlive ?? 0) as 0 | 1,
        isVisible: (values.isVisible ?? 1) as 0 | 1,
        // parentId 可能是空字符串/0，统一转 null（根菜单）
        parentId: values.parentId ?? null,
      } as MenuForm;
      if (id.value) {
        await menuApi.update(id.value, payload);
      } else {
        await menuApi.create(payload);
      }
      emits('success');
      drawerApi.close();
    } catch (error) {
      // 失败时解锁 drawer，让用户能继续操作/重试
      console.error('[MenuForm] submit failed', error);
      drawerApi.unlock();
    }
  },
  async onOpenChange(isOpen) {
    if (!isOpen) return;

    const data = drawerApi.getData<any>() ?? {};
    // 必须 await：resetForm 内部 getForm() + form.resetForm() 是链式 async，
    // 不 await 就直接 setValues 会导致后续的 merge 拿到上次的脏值（典型表现：type 默认成 3=按钮）
    await formApi.resetForm();
    id.value = undefined;
    await nextTick();

    if (data.id) {
      // === 修改菜单 ===
      id.value = data.id;
      // filterFields:false 跳过 deep merge，确保从干净状态赋值（避免上次的 icon/path 等残留）
      formApi.setValues(
        {
          parentId: data.parentId ?? null,
          type: data.type ?? 1,
          name: data.name,
          sort: data.sort ?? 0,
          icon: data.icon,
          path: data.path,
          component: data.component,
          permission: data.permission,
          routeParams: data.routeParams,
          status: data.status ?? 1,
          // 对齐后端字段名（camelCase）
          isExternal: data.isExternal ?? 0,
          isKeepAlive: data.isKeepAlive ?? 0,
          isVisible: data.isVisible ?? 1,
        },
        false,
      );
    } else if (data.parentId) {
      // === 新增子菜单：基于 parent.type 智能默认菜单类型 ===
      //   目录(type=1) 下新增 → 默认 菜单(type=2)（最常见的"目录→菜单"组合）
      //   菜单(type=2) 下新增 → 默认 按钮(type=3)（菜单下的操作权限）
      //   兜底 → 目录(type=1)
      const defaultType =
        data.parentType === 1
          ? 2
          : data.parentType === 2
            ? 3
            : 1;
      formApi.setValues(
        {
          parentId: data.parentId,
          type: defaultType,
          sort: 0,
          status: 1,
          isExternal: 0,
          isKeepAlive: 0,
          isVisible: 1,
        },
        false,
      );
    } else {
      // === 新增根菜单：默认目录类型 ===
      formApi.setValues(
        {
          type: 1,
          sort: 0,
          status: 1,
          isExternal: 0,
          isKeepAlive: 0,
          isVisible: 1,
        },
        false,
      );
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

<style scoped>
/*
 * 让 ant-design-vue InputNumber 的上/下箭头（.ant-input-number-handler-wrap）
 * 在 idle 状态下也常驻显示——默认是 opacity:0，只在 :hover / :focus 时才显示。
 * 用 :deep() 穿透 scoped 边界，影响 schema 里挂了 .always-show-controls 类的 InputNumber。
 */
:deep(.always-show-controls .ant-input-number-handler-wrap) {
  opacity: 1 !important;
}

/* 顺带让箭头本身在 idle 时颜色也清晰可见（默认 hover 状态颜色） */
:deep(.always-show-controls .ant-input-number-handler) {
  opacity: 1;
}
</style>
