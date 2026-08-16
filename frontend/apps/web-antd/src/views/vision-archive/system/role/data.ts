import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { RoleDto as SystemRole } from '#/api/vision-archive/system';

export function useColumns(
  onStatusChange?: (newStatus: any, row: SystemRole) => PromiseLike<boolean | undefined>,
): VxeTableGridColumns {
  return [
    {
      field: 'roleCode',
      title: '角色编号',
      width: 120,
    },
    {
      field: 'name',
      title: '角色名称',
      width: 160,
    },
    {
      field: 'code',
      title: '权限字符',
      width: 200,
    },
    {
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
        placeholder: '请输入权限字符',
      },
      fieldName: 'code',
      label: '权限字符',
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
      fieldName: 'roleCode',
      label: '角色编号',
      rules: 'required',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入权限字符',
      },
      fieldName: 'code',
      help: '控制器中定义的权限字符，如：system:user:list',
      label: '权限字符',
      rules: 'required',
    },
    {
      component: 'InputNumber',
      componentProps: {
        class: 'w-full always-show-controls',
        max: 5,
        min: 1,
        placeholder: '越小等级越高',
      },
      fieldName: 'level',
      help: '等级数值越小，权限等级越高（1 最高，5 最低）',
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
