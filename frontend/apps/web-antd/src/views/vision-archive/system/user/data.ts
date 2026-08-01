import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { BackendUserDto as SystemUser } from '#/api/vision-archive/system';

import { ROLE_OPTIONS, getOrgOptions } from '#/api/vision-archive/system';

export function useColumns(
  onStatusChange?: (newStatus: any, row: SystemUser) => PromiseLike<boolean | undefined>,
): VxeTableGridColumns {
  return [
    {
      field: 'id',
      title: '用户编号',
      width: 90,
    },
    {
      field: 'username',
      title: '账号',
      width: 140,
    },
    {
      field: 'realName',
      title: '姓名',
      width: 110,
    },
    {
      field: 'orgName',
      title: '部门',
      minWidth: 140,
    },
    {
      field: 'phoneNumber',
      title: '手机号码',
      width: 140,
    },
    {
      cellRender: {
        attrs: { beforeChange: onStatusChange },
        name: onStatusChange ? 'CellSwitch' : 'CellTag',
      },
      field: 'status',
      title: '状态',
      width: 90,
    },
    {
      field: 'createdAt',
      title: '创建时间',
      width: 170,
    },
    {
      align: 'center',
      field: 'operation',
      fixed: 'right',
      slots: { default: 'action' },
      title: '操作',
      width: 130,
    },
  ];
}

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'account',
      label: '账号',
      componentProps: {
        placeholder: '请输入账号',
        allowClear: true,
      },
    },
    {
      component: 'Input',
      fieldName: 'phone',
      label: '手机号码',
      componentProps: {
        placeholder: '请输入手机号码',
        allowClear: true,
      },
    },
    {
      component: 'Select',
      fieldName: 'status',
      label: '状态',
      componentProps: {
        allowClear: true,
        placeholder: '用户状态',
        options: [
          { label: '启用', value: 1 },
          { label: '禁用', value: 0 },
        ],
      },
    },
  ];
}

export function useFormSchema(orgOptions?: Array<{ label: string; value: string }>): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'username',
      label: '账号',
      rules: 'required',
    },
    {
      component: 'Input',
      fieldName: 'name',
      label: '姓名',
      rules: 'required',
    },
    {
      component: 'RadioGroup',
      fieldName: 'gender',
      label: '性别',
      componentProps: {
        buttonStyle: 'solid',
        optionType: 'button',
        options: [
          { label: '男', value: 'male' },
          { label: '女', value: 'female' },
          { label: '未知', value: '' },
        ],
      },
    },
    {
      component: 'Select',
      fieldName: 'role',
      label: '角色',
      rules: 'required',
      componentProps: {
        allowClear: true,
        options: ROLE_OPTIONS,
      },
    },
    {
      component: 'Select',
      fieldName: 'org',
      label: '部门',
      rules: 'required',
      componentProps: {
        allowClear: true,
        options: orgOptions ?? getOrgOptions(),
      },
    },
    {
      component: 'Input',
      fieldName: 'phone',
      label: '手机号码',
    },
    {
      component: 'Input',
      fieldName: 'email',
      label: '邮箱',
    },
    {
      component: 'InputPassword',
      fieldName: 'password',
      label: '密码',
      componentProps: {
        placeholder: '编辑时留空则不修改',
      },
    },
    {
      component: 'RadioGroup',
      fieldName: 'status',
      label: '状态',
      componentProps: {
        buttonStyle: 'solid',
        optionType: 'button',
        defaultValue: 'active',
        options: [
          { label: '启用', value: 'active' },
          { label: '禁用', value: 'inactive' },
        ],
      },
    },
  ];
}
