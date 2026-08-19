import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';

// =====================================================================
// 操作日志 DTO
// =====================================================================

/**
 * 后端返回的原始操作日志
 * 字段命名以前端约定为准（id/operator/type/module/content/ip/status/costMs/createdAt），
 * 后续真正接到后端时，如字段名不一致只需在 API 层做一次映射。
 */
export interface OperationLogDto {
  id: number;
  /** 操作人账号/姓名 */
  operator: string;
  /** 操作类型: login / create / update / delete / export / import / query / other */
  type: string;
  /** 操作模块: 系统管理 / 学生档案 / 数据采集 ... */
  module: string;
  /** 操作内容（自然语言描述） */
  content: string;
  /** 客户端 IP */
  ip: string;
  /** 1=成功 0=失败 */
  status: 0 | 1;
  /** 耗时（毫秒） */
  costMs: number;
  /** 操作时间 ISO 字符串 */
  createdAt: string;
  /** 浏览器 UA（详情里展示，可选） */
  userAgent?: string;
  /** 登录地点（可选） */
  location?: string;
}

// =====================================================================
// 字典 / 颜色映射
// =====================================================================

/** 操作类型 -> 展示文本 + 标签颜色（用于 vxe CellTag） */
export const LOG_TYPE_MAP: Record<string, { text: string; color: string }> = {
  login: { text: '登录', color: 'blue' },
  logout: { text: '登出', color: 'default' },
  create: { text: '新增', color: 'green' },
  update: { text: '修改', color: 'orange' },
  delete: { text: '删除', color: 'red' },
  export: { text: '导出', color: 'purple' },
  import: { text: '导入', color: 'cyan' },
  query: { text: '查询', color: 'geekblue' },
  other: { text: '其它', color: 'default' },
};

/** 操作类型下拉选项（全部 + 各业务类型） */
export const LOG_TYPE_OPTIONS = [
  { label: '全部', value: '' },
  { label: '登录', value: 'login' },
  { label: '登出', value: 'logout' },
  { label: '新增', value: 'create' },
  { label: '修改', value: 'update' },
  { label: '删除', value: 'delete' },
  { label: '导出', value: 'export' },
  { label: '导入', value: 'import' },
  { label: '查询', value: 'query' },
  { label: '其它', value: 'other' },
];

/** 状态下拉选项 */
export const LOG_STATUS_OPTIONS = [
  { label: '全部', value: '' },
  { label: '成功', value: 1 },
  { label: '失败', value: 0 },
];

// =====================================================================
// 表格列定义
// =====================================================================

export function useColumns(): VxeTableGridColumns {
  return [
    { field: 'id', title: '编号', width: 90 },
    { field: 'operator', title: '操作人', width: 110 },
    {
      field: 'type',
      title: '操作类型',
      width: 100,
      cellRender: {
        name: 'CellTag',
        // 通过 options 让 CellTag 用字典里的颜色
        options: Object.entries(LOG_TYPE_MAP).map(([value, { text, color }]) => ({
          value,
          label: text,
          color,
        })),
      },
    },
    { field: 'module', title: '操作模块', width: 130 },
    {
      field: 'content',
      title: '操作内容',
      minWidth: 240,
      slots: { default: 'content' },
    },
    {
      field: 'ip',
      title: 'IP 地址',
      width: 170,
      // IPv4-mapped IPv6（::ffff:192.168.1.1）这种最长 ~22 字符，130px 不够；
      // 改 170 留出 padding，hover 用 tooltip 看全
      showOverflow: 'tooltip',
    },
    {
      field: 'status',
      title: '状态',
      width: 90,
      align: 'center',
      // 后端约定：1 = 成功，0 = 失败；前端用 CellTag 渲染成"成功/失败"色块
      // （与详情弹框里 detailStatus 的 1→正常/0→异常 含义保持一致）
      cellRender: {
        name: 'CellTag',
        options: [
          { value: 0, label: '失败', color: 'red' },
          { value: 1, label: '成功', color: 'green' },
        ],
      },
    },
    {
      field: 'costMs',
      title: '耗时',
      width: 90,
      formatter: ({ cellValue }: { cellValue: number }) =>
        cellValue == null ? '—' : `${cellValue} ms`,
    },
    {
      field: 'createdAt',
      title: '操作时间',
      minWidth: 170,
      sortable: true,
    },
    {
      align: 'center',
      field: 'operation',
      fixed: 'right',
      slots: { default: 'action' },
      title: '操作',
      width: 80,
    },
  ];
}

// =====================================================================
// 顶部筛选表单
// =====================================================================

export function useGridFormSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入操作人',
      },
      fieldName: 'operator',
      label: '操作人',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: LOG_TYPE_OPTIONS,
        placeholder: '操作类型',
      },
      fieldName: 'type',
      label: '操作类型',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入模块名称',
      },
      fieldName: 'module',
      label: '操作模块',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入 IP 地址',
      },
      fieldName: 'ip',
      label: 'IP 地址',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: LOG_STATUS_OPTIONS,
        placeholder: '操作结果',
      },
      fieldName: 'status',
      label: '状态',
    },
    {
      component: 'RangePicker',
      fieldName: 'createdAtRange',
      label: '操作时间',
      componentProps: {
        valueFormat: 'YYYY-MM-DD',
      },
    },
  ];
}
