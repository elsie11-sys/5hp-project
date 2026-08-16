import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { MenuDto as SystemMenu } from '#/api/vision-archive/system';

import { menuApi } from '#/api/vision-archive/system';

/** 菜单类型枚举（与后端 MenuDto.type 一致：1=目录 2=菜单 3=按钮） */
const MENU_TYPE_OPTIONS = [
  { label: '目录', value: 1 },
  { label: '菜单', value: 2 },
  { label: '按钮', value: 3 },
];

/** 状态文案/颜色（1=正常 0=停用） */
const STATUS_TEXT_MAP: Record<number, string> = {
  1: '启用',
  0: '停用',
};
const STATUS_COLOR_MAP: Record<number, string> = {
  1: 'green',
  0: 'red',
};

export function useColumns(): VxeTableGridColumns {
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
      align: 'center',
      field: 'sort',
      title: '排序',
      width: 60,
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
      minWidth: 220,
    },
    {
      cellRender: {
        attrs: {
          colorMap: STATUS_COLOR_MAP,
          textMap: STATUS_TEXT_MAP,
        },
        name: 'CellTag',
      },
      field: 'status',
      title: '状态',
      width: 80,
    },
    {
      align: 'center',
      field: 'createdAt',
      formatter: 'formatDateTime',
      title: '创建时间',
      width: 180,
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
        placeholder: '请输入',
      },
      fieldName: 'name',
      label: '菜单名称',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: [
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
        placeholder: '菜单状态',
      },
      fieldName: 'status',
      label: '状态',
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

/** 菜单类型判断（与 vben-v2 一致，编号按当前项目：1=目录 2=菜单 3=按钮） */
const isDir = (type: number | string | undefined) => type === 1;
const isMenu = (type: number | string | undefined) => type === 2;
const isButton = (type: number | string | undefined) => type === 3;

export function useFormSchema(): VbenFormSchema[] {
  return [
    // ===== Row 1: 上级菜单（全宽） =====
    {
      component: 'ApiTreeSelect',
      componentProps: {
        allowClear: true,
        api: async () => {
          const tree = await menuApi.getTree();
          return buildTreeOptions(tree);
        },
        labelField: 'label',
        placeholder: '请选择上级菜单',
        treeDefaultExpandAll: true,
        valueField: 'value',
      },
      fieldName: 'parentId',
      formItemClass: 'md:cols-span-full',
      label: '上级菜单',
    },

    // ===== Row 2: 菜单类型（全宽，普通 radio） =====
    {
      component: 'RadioGroup',
      componentProps: {
        options: MENU_TYPE_OPTIONS,
      },
      defaultValue: 1,
      fieldName: 'type',
      formItemClass: 'md:cols-span-full',
      label: '菜单类型',
    },

    // ===== Row 3: 菜单图标（全宽，按钮类型不显示） =====
    {
      component: 'IconPicker',
      componentProps: {
        placeholder: '点击选择图标',
      },
      dependencies: {
        if: (values) => !isButton(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'icon',
      formItemClass: 'md:cols-span-full',
      label: '菜单图标',
    },

    // ===== Row 4: 菜单名称 * | 显示排序 * =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入菜单名称',
      },
      fieldName: 'name',
      label: '菜单名称',
      rules: 'required',
    },
    {
      component: 'InputNumber',
      componentProps: {
        // always-show-controls 标记类 + 下面的 scoped CSS 让上/下箭头常驻显示
        class: 'w-full always-show-controls',
        max: 999,
        min: 0,
        placeholder: '请输入显示排序',
      },
      defaultValue: 0,
      fieldName: 'sort',
      label: '显示排序',
      rules: 'required',
    },

    // ===== Row 5: 是否外链 | 路由地址 *（按钮不显示） =====
    {
      component: 'RadioGroup',
      componentProps: {
        options: [
          { label: '是', value: 1 },
          { label: '否', value: 0 },
        ],
      },
      defaultValue: 0,
      dependencies: {
        if: (values) => !isButton(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'isExternal',
      help: '是否外链，是则点击菜单跳转到外网',
      label: '是否外链',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '如：user',
      },
      dependencies: {
        if: (values) => !isButton(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'path',
      help: '访问的路由地址，例如 user',
      label: '路由地址',
      rules: 'required',
    },

    // ===== Row 6: 组件路径（仅菜单） | 权限字符（菜单/按钮） =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '如：system/user/index',
      },
      dependencies: {
        if: (values) => isMenu(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'component',
      help: '组件的完整路径，相对于 src/views 目录',
      label: '组件路径',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '如：system:user:list',
      },
      dependencies: {
        if: (values) => !isDir(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'permission',
      help: '权限字符，例如 system:user:list',
      label: '权限字符',
    },

    // ===== Row 7: 路由参数 | 是否缓存（仅菜单，按钮不显示） =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入路由参数',
      },
      dependencies: {
        if: (values) => !isButton(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'routeParams',
      help: '可选，路由的 query 参数，JSON 字符串',
      label: '路由参数',
    },
    {
      component: 'RadioGroup',
      componentProps: {
        options: [
          { label: '缓存', value: 1 },
          { label: '不缓存', value: 0 },
        ],
      },
      defaultValue: 0,
      dependencies: {
        if: (values) => isMenu(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'isKeepAlive',
      help: '切换页面时是否缓存该组件',
      label: '是否缓存',
    },

    // ===== Row 8: 显示状态（按钮不显示） | 菜单状态 =====
    {
      component: 'RadioGroup',
      componentProps: {
        options: [
          { label: '显示', value: 1 },
          { label: '隐藏', value: 0 },
        ],
      },
      defaultValue: 1,
      dependencies: {
        if: (values) => !isButton(values.type),
        triggerFields: ['type'],
      },
      fieldName: 'isVisible',
      help: '菜单在侧边栏/面包屑中是否显示',
      label: '显示状态',
    },
    {
      component: 'RadioGroup',
      componentProps: {
        options: [
          { label: '正常', value: 1 },
          { label: '停用', value: 0 },
        ],
      },
      defaultValue: 1,
      fieldName: 'status',
      help: '停用后菜单不显示且无法访问',
      label: '菜单状态',
      rules: 'required',
    },
  ];
}
