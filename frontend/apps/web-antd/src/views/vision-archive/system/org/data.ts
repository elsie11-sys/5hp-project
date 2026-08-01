import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { OrgDto as SystemOrg } from '#/api/vision-archive/system';

import { ORG_LEVEL_OPTIONS } from '#/api/vision-archive/system';

export function useColumns(
  onStatusChange?: (newStatus: any, row: SystemOrg) => PromiseLike<boolean | undefined>,
): VxeTableGridColumns {
  return [
    {
      field: 'name',
      title: '组织名称',
      width: 180,
    },
    {
      field: 'code',
      title: '组织编码',
      width: 140,
    },
    {
      cellRender: {
        name: 'CellTag',
      },
      field: 'level',
      title: '级别',
      width: 100,
    },
    {
      field: 'sort',
      title: '排序',
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
      label: '组织名称',
    },
    {
      component: 'Select',
      fieldName: 'level',
      label: '级别',
      componentProps: {
        allowClear: true,
        options: ORG_LEVEL_OPTIONS,
      },
    },
  ];
}

export function useFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'name',
      label: '组织名称',
      rules: 'required',
    },
    {
      component: 'Input',
      fieldName: 'code',
      label: '组织编码',
    },
    {
      component: 'Select',
      fieldName: 'level',
      label: '级别',
      rules: 'required',
      componentProps: {
        options: ORG_LEVEL_OPTIONS,
      },
    },
    {
      component: 'InputNumber',
      fieldName: 'sort',
      label: '排序',
      componentProps: {
        min: 0,
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
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
      },
    },
  ];
}
