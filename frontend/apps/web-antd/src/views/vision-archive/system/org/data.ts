import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';
import type { OrgDto as SystemOrg } from '#/api/vision-archive/system';

import { dictApi, orgApi } from '#/api/vision-archive/system';

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
 *
 * 注意：key 是字典 itemValue（数字字符串"1"~"5"），不是中文标签。
 * 数据库 sys_org.level 存的就是 itemValue，渲染时按 itemValue 查色。
 */
const LEVEL_COLOR_MAP: Record<string, string> = {
  '1': 'red',
  '2': 'orange',
  '3': 'blue',
  '4': 'cyan',
  '5': 'green',
};

/**
 * 字典未加载好时，渲染阶段的最后兜底。
 * 同样按 itemValue 查中文 label，避免 row.level="1" 时给用户看个光秃秃数字。
 */
const LEVEL_LABEL_MAP: Record<string, string> = {
  '1': '国家级',
  '2': '省级',
  '3': '市级',
  '4': '区县级',
  '5': '学校级',
};

/**
 * 组织级别兜底选项（按国家级→学校级从高到低）。
 *
 * 数据源是字典 sys_org_level，存储时存「字典项值」(itemValue)，如 "1" 代表国家级。
 * 当字典接口拉不到/字典里没配置时，用这个硬编码兜底，保证表单可用。
 */
const ORG_LEVEL_FALLBACK: Array<{ label: string; value: string }> = [
  { label: '国家级', value: '1' },
  { label: '省级', value: '2' },
  { label: '市级', value: '3' },
  { label: '区县级', value: '4' },
  { label: '学校级', value: '5' },
];

/** 字典类型常量（统一一处引用，避免拼写漂移） */
export const ORG_LEVEL_DICT_TYPE = 'sys_org_level';

/**
 * 拉取组织级别字典项，转为 {label, value}。
 * 业务约定：label = itemLabel（显示用），value = itemValue（提交到后端 sys_org.level）。
 * 数据库存 itemValue（如 "1"），前端通过字典映射显示成 itemLabel（如"国家级"）。
 *
 * 拉不到 / 字典为空 → 兜底为 ORG_LEVEL_FALLBACK。
 */
export async function loadOrgLevelOptions(): Promise<
  Array<{ label: string; value: string }>
> {
  try {
    const dict = await dictApi.getByType(ORG_LEVEL_DICT_TYPE);
    const items = (dict?.dictItems ?? [])
      .filter((it) => it.status === 1 || it.status === undefined)
      .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
      .map((it) => ({ label: it.itemLabel, value: it.itemValue }));
    return items.length > 0 ? items : ORG_LEVEL_FALLBACK;
  } catch {
    return ORG_LEVEL_FALLBACK;
  }
}

// =====================================================================
// 列表渲染用的字典缓存（模块级单例）
// =====================================================================

/** 字典项在内存里的表达：label 用于展示，color 用于 Tag 配色 */
type LevelEntry = { label: string; color: string };

/**
 * 缓存：[itemValue → {label, color}]
 *
 * 数据库 sys_org.level 存的是字典 itemValue（如 "1"），所以 key 用 itemValue。
 * 列表渲染时 row.level 作为 key 查表，找不到就 fallback 到 LEVEL_LABEL_MAP / LEVEL_COLOR_MAP。
 */
const levelMap: Record<string, LevelEntry> = {};

/** 单次加载 Promise 句柄；重复调用 ensureLevelDictLoaded 复用同一个 promise，避免每次 query 都打接口 */
let loadLevelDictPromise: Promise<void> | null = null;

/**
 * 异步加载组织级别字典到本地缓存。
 * 必须在 grid query 之前 await，确保渲染时缓存就绪。
 */
export function ensureLevelDictLoaded(): Promise<void> {
  if (!loadLevelDictPromise) {
    loadLevelDictPromise = (async () => {
      try {
        const opts = await loadOrgLevelOptions();
        // 先清空，避免字典项删减后旧 key 残留
        Object.keys(levelMap).forEach((k) => delete levelMap[k]);
        for (const o of opts) {
          levelMap[o.value] = {
            label: o.label,
            color: LEVEL_COLOR_MAP[o.value] ?? 'default',
          };
        }
      } catch {
        // 静默失败：getLevelLabel/getLevelColor 会 fallback 到 LEVEL_LABEL_MAP / LEVEL_COLOR_MAP
      }
    })();
  }
  return loadLevelDictPromise;
}

/**
 * 同步查字典 label。
 * 优先级：levelMap（动态字典）→ LEVEL_LABEL_MAP（硬编码兜底）→ rawLevel 本身。
 */
export function getLevelLabel(rawLevel: string | undefined): string {
  if (!rawLevel) return '';
  return (
    levelMap[rawLevel]?.label ?? LEVEL_LABEL_MAP[rawLevel] ?? rawLevel
  );
}

/**
 * 同步查字典 color。
 * 优先级：levelMap（动态字典）→ LEVEL_COLOR_MAP（硬编码兜底）→ 'default'。
 */
export function getLevelColor(rawLevel: string | undefined): string {
  if (!rawLevel) return 'default';
  return (
    levelMap[rawLevel]?.color ?? LEVEL_COLOR_MAP[rawLevel] ?? 'default'
  );
}

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

    // ===== Row 2: 组织级别（全宽，ApiSelect 拉 sys_org_level 字典项）=====
    // 业务约定：value 存的是字典项的 itemValue（如 "1" 代表国家级），
    // 提交时 values.level 落到后端 sys_org.level，前端通过字典映射显示成 itemLabel。
    {
      component: 'ApiSelect',
      componentProps: {
        allowClear: true,
        api: loadOrgLevelOptions,
        // 字典项少（国家级→学校级），下拉打开时拉一次即可，不必 alwaysLoad
        immediate: true,
        placeholder: '请选择组织级别',
      },
      defaultValue: '1',
      fieldName: 'level',
      formItemClass: 'md:cols-span-full',
      label: '组织级别',
      rules: 'required',
    },

    // ===== Row 3: 部门 | 显示排序 =====
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

    // ===== Row 4: 负责人 | 联系电话 =====
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

    // ===== Row 5: 邮箱 | 部门状态 =====
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
