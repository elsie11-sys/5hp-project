import type { OnActionClickFn, VxeTableGridColumns } from '#/adapter/vxe-table';
import type { SystemMenuApi } from '#/api/system/menu';

export function useColumns(
  onActionClick: OnActionClickFn<SystemMenuApi.SystemMenu>,
): VxeTableGridColumns<SystemMenuApi.SystemMenu> {
  return [
    {
      align: 'left',
      field: 'name',
      fixed: 'left',
      slots: { default: 'title' },
      title: '菜单名称',
      treeNode: true,
      width: 200,
    },
    {
      align: 'center',
      field: 'icon',
      title: '图标',
      width: 80,
      slots: { default: 'icon' },
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
      width: 200,
    },
    {
      align: 'left',
      field: 'component',
      title: '组件路径',
      width: 200,
    },
    {
      cellRender: { name: 'CellTag' },
      field: 'status',
      title: '状态',
      width: 80,
    },
    {
      align: 'left',
      field: 'createdAt',
      title: '创建时间',
      width: 160,
    },
    {
      align: 'right',
      cellRender: {
        attrs: {
          nameField: 'name',
          onClick: onActionClick,
        },
        name: 'CellOperation',
        options: [
          {
            code: 'edit',
            icon: 'carbon:edit',
            text: '修改',
          },
          {
            code: 'append',
            icon: 'carbon:add',
            text: '新增',
          },
          {
            code: 'delete',
            icon: 'carbon:trash-can',
            text: '删除',
          },
        ],
      },
      field: 'operation',
      fixed: 'right',
      headerAlign: 'center',
      showOverflow: false,
      title: '操作',
      width: 220,
    },
  ];
}

export function useSearchFormSchema() {
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
      componentProps: {
        allowClear: true,
        placeholder: '菜单状态',
        options: SystemMenuApi.MenuStatus.map((item) => ({
          label: item.label,
          value: item.value,
        })),
      },
      fieldName: 'status',
      label: '菜单状态',
    },
  ];
}
