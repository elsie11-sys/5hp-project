import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { MenuDto as SystemMenu } from '#/api/vision-archive/system';

import { menuApi } from '#/api/vision-archive/system';

const MENU_TYPE_OPTIONS = [
  { label: '目录', value: 1 },
  { label: '菜单', value: 2 },
  { label: '按钮', value: 3 },
];

const MENU_TYPE_COLOR_MAP: Record<number, string> = {
  1: 'default',
  2: 'processing',
  3: 'warning',
};

const MENU_TYPE_TEXT_MAP: Record<number, string> = {
  1: '目录',
  2: '菜单',
  3: '按钮',
};

export function useColumns(
  onStatusChange?: (newStatus: any, row: SystemMenu) => PromiseLike<boolean | undefined>,
): VxeTableGridColumns {
  return [
    {
      align: 'left',
      field: 'name',
      fixed: 'left',
      slots: { default: 'title' },
      title: '菜单名称',
      treeNode: true,
      width: 220,
    },
    {
      align: 'center',
      field: 'icon',
      slots: { default: 'icon' },
      title: '图标',
      width: 80,
    },
    {
      cellRender: {
        name: 'CellTag',
        attrs: {
          colorMap: MENU_TYPE_COLOR_MAP,
          textMap: MENU_TYPE_TEXT_MAP,
        },
      },
      field: 'type',
      title: '类型',
      width: 80,
    },
    {
      align: 'center',
      field: 'sort',
      title: '排序',
      width: 80,
    },
    {
      align: 'left',
      field: 'permission',
      title: '权限标识',
      width: 180,
    },
    {
      align: 'left',
      field: 'component',
      title: '组件路径',
      width: 180,
    },
    {
      cellRender: {
        attrs: { beforeChange: onStatusChange },
        name: onStatusChange ? 'CellSwitch' : 'CellTag',
      },
      field: 'status',
      title: '状态',
      width: 100,
    },
    {
      align: 'center',
      field: 'operation',
      fixed: 'right',
      slots: { default: 'action' },
      title: '操作',
      width: 220,
    },
  ];
}

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入菜单名称',
      },
      fieldName: 'name',
      label: '菜单名称',
    },
    {
      component: 'Select',
      fieldName: 'status',
      label: '状态',
      componentProps: {
        allowClear: true,
        options: [
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
      },
    },
  ];
}

/** 将后端菜单树转为 TreeSelect 所需的 {label,value,children} 结构（过滤掉按钮类型） */
function buildTreeOptions(list: SystemMenu[] = []): any[] {
  return list
    .filter((item) => item.type !== 3)
    .map((item) => ({
      label: item.name,
      value: item.id,
      children: item.children?.length
        ? buildTreeOptions(item.children)
        : undefined,
    }));
}

export function useFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'ApiTreeSelect',
      componentProps: {
        allowClear: true,
        placeholder: '请选择上级菜单（留空为根级）',
        treeDefaultExpandAll: true,
        api: async () => {
          const tree = await menuApi.getTree();
          return buildTreeOptions(tree);
        },
        labelField: 'label',
        valueField: 'value',
      },
      fieldName: 'parentId',
      label: '上级菜单',
    },
    {
      component: 'RadioGroup',
      componentProps: {
        buttonStyle: 'solid',
        options: MENU_TYPE_OPTIONS,
        optionType: 'button',
      },
      defaultValue: 1,
      fieldName: 'type',
      label: '菜单类型',
      rules: 'required',
    },
    {
      component: 'IconPicker',
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
      component: 'Input',
      componentProps: {
        placeholder: '请输入路由地址',
      },
      dependencies: {
        if: (values) => values.type !== 3,
        triggerFields: ['type'],
      },
      fieldName: 'path',
      label: '路由地址',
    },
    {
      component: 'Input',
      componentProps: {
        placeholder: '请输入组件路径',
      },
      dependencies: {
        if: (values) => values.type !== 3,
        triggerFields: ['type'],
      },
      fieldName: 'component',
      label: '组件路径',
    },
    {
      component: 'Input',
      componentProps: {
        placeholder: '如：system:user:list',
      },
      fieldName: 'permission',
      label: '权限标识',
    },
    {
      component: 'RadioGroup',
      componentProps: {
        buttonStyle: 'solid',
        optionType: 'button',
        options: [
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
      },
      defaultValue: 1,
      fieldName: 'status',
      label: '菜单状态',
      rules: 'required',
    },
  ];
}
