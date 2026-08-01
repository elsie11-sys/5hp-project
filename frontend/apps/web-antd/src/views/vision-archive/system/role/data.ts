import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { RoleDto as SystemRole } from '#/api/vision-archive/system';

import { ROLE_LEVEL_OPTIONS } from '#/api/vision-archive/system';

export function useColumns(
  onStatusChange?: (newStatus: any, row: SystemRole) => PromiseLike<boolean | undefined>,
): VxeTableGridColumns {
  return [
    {
      field: 'name',
      title: '角色名称',
      width: 160,
    },
    {
      field: 'code',
      title: '角色编码',
      width: 160,
    },
    {
      cellRender: {
        name: 'CellTag',
        attrs: {
          colorMap: { 1: 'green', 2: 'blue', 3: 'orange', 4: 'purple', 5: 'cyan' },
        },
      },
      field: 'level',
      title: '等级',
      width: 80,
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
      field: 'remark',
      title: '备注',
      minWidth: 120,
    },
    {
      field: 'createdAt',
      title: '创建时间',
      width: 180,
    },
    {
      align: 'center',
      field: 'operation',
      fixed: 'right',
      slots: { default: 'action' },
      title: '操作',
      width: 180,
    },
  ];
}

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'name',
      label: '角色名称',
    },
    {
      component: 'Select',
      fieldName: 'status',
      label: '状态',
      componentProps: {
        allowClear: true,
        options: [
          { label: '启用', value: 1 },
          { label: '禁用', value: 0 },
        ],
      },
    },
  ];
}

export function useFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'name',
      label: '角色名称',
      rules: 'required',
    },
    {
      component: 'Input',
      fieldName: 'code',
      label: '角色编码',
      rules: 'required',
    },
    {
      component: 'Select',
      fieldName: 'level',
      label: '等级',
      rules: 'required',
      componentProps: {
        options: ROLE_LEVEL_OPTIONS,
      },
    },
    {
      component: 'RadioGroup',
      fieldName: 'status',
      label: '状态',
      componentProps: {
        buttonStyle: 'solid',
        optionType: 'button',
        defaultValue: 1,
        options: [
          { label: '启用', value: 1 },
          { label: '禁用', value: 0 },
        ],
      },
    },
    {
      component: 'Textarea',
      fieldName: 'remark',
      label: '备注',
    },
  ];
}
