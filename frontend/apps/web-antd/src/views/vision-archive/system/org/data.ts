import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { OrgDto as SystemOrg } from '#/api/vision-archive/system';

import { orgApi } from '#/api/vision-archive/system';

/** 状态文案/颜色（1=启用 0=停用），按截图样式 */
const STATUS_TEXT_MAP: Record<number, string> = {
  1: '启用',
  0: '停用',
};
const STATUS_COLOR_MAP: Record<number, string> = {
  1: 'green',
  0: 'red',
};

/**
 * 组织级别配色（国家级最深，往下逐级变浅）
 * 树表里用 Tag 挂在组织名前，一眼能看出是几级机构
 */
const LEVEL_COLOR_MAP: Record<string, string> = {
  国家级: 'red',
  省级: 'orange',
  市级: 'blue',
  区县级: 'cyan',
  学校级: 'green',
};

export function useColumns(): VxeTableGridColumns {
  return [
    {
      align: 'left',
      field: 'name',
      fixed: 'left',
      slots: { default: 'name' },
      title: '组织名称',
      treeNode: true,
      minWidth: 260,
    },
    {
      align: 'center',
      field: 'sort',
      title: '排序',
      width: 80,
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
      align: 'left',
      field: 'remark',
      minWidth: 160,
      title: '备注',
    },
    {
      align: 'center',
      field: 'operation',
      fixed: 'right',
      slots: { default: 'action' },
      title: '操作',
      width: 200,
    },
  ];
}

export { LEVEL_COLOR_MAP, STATUS_COLOR_MAP, STATUS_TEXT_MAP };

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入',
      },
      fieldName: 'name',
      label: '组织名称',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: [
          { label: '启用', value: 1 },
          { label: '停用', value: 0 },
        ],
        placeholder: '请选择',
      },
      fieldName: 'status',
      label: '状态',
    },
  ];
}

export function useFormSchema(): VbenFormSchema[] {
  return [
    // ===== Row 1: 上级（全宽，ApiTreeSelect 拉组织树）=====
    {
      component: 'ApiTreeSelect',
      componentProps: {
        allowClear: true,
        api: async () => {
          const tree = await orgApi.getTree();
          return buildOrgTreeOptions(tree);
        },
        // ant-design-vue TreeSelect 默认 treeNodeFilterProp='value'（即按 ID 匹配），
        // 用户在搜索框里输入组织名称时根本匹配不到；这里改成 'label'，按名称模糊匹配
        treeNodeFilterProp: 'label',
        labelField: 'label',
        placeholder: '选择上级',
        showSearch: true,
        treeDefaultExpandAll: true,
        valueField: 'value',
      },
      fieldName: 'parentId',
      formItemClass: 'md:cols-span-full',
      label: '上级',
    },

    // ===== Row 2: 部门 | 显示排序 =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入部门',
      },
      fieldName: 'name',
      label: '部门',
      rules: 'required',
    },
    {
      component: 'InputNumber',
      componentProps: {
        // always-show-controls 标记类 + form.vue 里的 :deep() CSS 让上/下箭头常驻显示
        class: 'w-full always-show-controls',
        min: 0,
        placeholder: '请输入',
      },
      defaultValue: 0,
      fieldName: 'sort',
      label: '显示排序',
      rules: 'required',
    },

    // ===== Row 3: 负责人 | 联系电话 =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入负责人',
      },
      fieldName: 'leader',
      label: '负责人',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入联系电话',
      },
      fieldName: 'phone',
      label: '联系电话',
    },

    // ===== Row 4: 邮箱 | 部门状态 =====
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入邮箱',
      },
      fieldName: 'email',
      label: '邮箱',
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
      label: '部门状态',
    },
  ];
}

/** 将后端组织树转为 TreeSelect 所需的 {label,value,children} 结构 */
function buildOrgTreeOptions(
  list: SystemOrg[] = [],
): Array<{ label: string; value: number; children?: any[] }> {
  return list.map((item) => ({
    label: item.name,
    value: item.id,
    children: item.children?.length
      ? buildOrgTreeOptions(item.children)
      : undefined,
  }));
}
