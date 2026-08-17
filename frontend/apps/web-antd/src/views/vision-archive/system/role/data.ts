import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';

import {
  ROLE_LEVEL_MAP,
  type RoleDto as SystemRole,
} from '#/api/vision-archive/system';

export function useColumns(opts?: {
  onStatusChange?: (newStatus: number, row: SystemRole) => Promise<boolean> | boolean;
}): VxeTableGridColumns {
  return [
    {
      field: 'code',
      title: '角色编号',
      width: 200,
    },
    {
      field: 'name',
      title: '角色名称',
      width: 160,
    },
    {
      field: 'permission',
      title: '权限字符',
      width: 200,
    },
    {
      // 等级：1~5 显示标准名称，其它数值回退到 `等级 N` 让用户看清实际值
      field: 'level',
      title: '等级',
      width: 100,
      formatter: ({ cellValue }: { cellValue: number }) =>
        ROLE_LEVEL_MAP[cellValue] ?? `等级 ${cellValue}`,
    },
    {
      // 状态列：用 vben 的 CellSwitch 渲染（注册在 adapter/vxe-table.ts）
      // 之前 data.ts 这里用 `name: onStatusChange ? 'CellSwitch' : 'CellTag'`，
      // 但 onStatusChange 在本模块作用域里根本不存在 → name 永远是 'CellTag'，
      // 状态列被渲染成 Tag 而非 Switch，DB 里 status=1 也就显示不对。
      // 改成接受 opts.onStatusChange 后显式传 beforeChange，并固定 name='CellSwitch'。
      cellRender: {
        attrs: { beforeChange: opts?.onStatusChange },
        props: { activeValue: 1, inactiveValue: 0 },
        name: 'CellSwitch',
      },
      field: 'status',
      title: '状态',
      width: 100,
    },
    {
      field: 'remark',
      title: '备注',
      minWidth: 100,
      slots: { default: 'remark' },
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
      width: 360,
    },
  ];
}

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入角色名称',
      },
      fieldName: 'name',
      label: '角色名称',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入角色编号/权限字符',
      },
      fieldName: 'code',
      label: '角色编号',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: [
          { label: '启用', value: 1 },
          { label: '禁用', value: 0 },
        ],
        placeholder: '角色状态',
      },
      fieldName: 'status',
      label: '状态',
    },
    {
      component: 'RangePicker',
      fieldName: 'createdAtRange',
      label: '创建时间',
      // 把 [start, end] 拆成 startDate/endDate 给后端 RoleQuery 用
      componentProps: {
        valueFormat: 'YYYY-MM-DD',
      },
    },
  ];
}

export function useFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入角色名称',
      },
      fieldName: 'name',
      label: '角色名称',
      rules: 'required',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入角色编号',
      },
      fieldName: 'code',
      help: '角色业务标识，全局唯一，如：ROLE_NATIONAL',
      label: '角色编号',
      rules: 'required',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入权限字符',
      },
      fieldName: 'permission',
      help: '控制器/注解中使用的权限标识，如：admin / system:user:list',
      label: '权限字符',
    },
    {
      component: 'InputNumber',
      componentProps: {
        class: 'w-full always-show-controls',
        min: 1,
        precision: 0,
        placeholder: '请输入等级（1 最高，数字越大权限越低）',
      },
      fieldName: 'level',
      defaultValue: 5,
      help: '1~5 对应 国家级/省级/市级/区县级/学校级；如需更细粒度可填 ≥6 的整数',
      label: '等级',
      rules: 'required',
    },
    {
      component: 'RadioGroup',
      fieldName: 'status',
      formItemClass: 'role-status-radio',
      label: '状态',
      defaultValue: 1,
      componentProps: {
        options: [
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
      },
    },
    {
      component: 'Textarea',
      componentProps: {
        allowClear: true,
        placeholder: '请输入内容',
        rows: 3,
      },
      fieldName: 'remark',
      label: '备注',
    },
  ];
}
