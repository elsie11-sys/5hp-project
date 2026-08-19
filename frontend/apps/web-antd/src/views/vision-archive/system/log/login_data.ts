import type { VbenFormSchema } from '#/adapter/form';
import type { VxeTableGridColumns } from '#/adapter/vxe-table';

// =====================================================================
// 登录日志 DTO（与 api/vision-archive/system.ts 中的 LoginLogDto 保持一致）
// 这里再写一份仅为视图层自给自足，方便单文件维护
// =====================================================================

export interface LoginLogDto {
  id: number;
  /** 用户名称 */
  userName: string;
  /** 登录地址（IP） */
  ip: string;
  /** 登录地点 */
  location: string;
  /** 操作系统 */
  os: string;
  /** 浏览器 */
  browser: string;
  /** 登录状态：1=成功 0=失败 */
  status: 0 | 1;
  /** 描述（登录成功/失败原因） */
  message: string;
  /** 访问时间（登录时间） */
  loginTime: string;
}

// =====================================================================
// 字典 / 颜色映射
// =====================================================================

/** 登录状态下拉选项 */
export const LOGIN_STATUS_OPTIONS = [
  { label: '全部', value: '' },
  { label: '成功', value: 1 },
  { label: '失败', value: 0 },
];

// =====================================================================
// 表格列定义
// =====================================================================

export function useColumns(): VxeTableGridColumns {
  return [
    {
      field: 'id',
      title: '访问编号',
      width: 90,
      align: 'center',
    },
    {
      field: 'userName',
      title: '用户名称',
      width: 130,
    },
    {
      field: 'ip',
      title: '地址',
      width: 150,
      showOverflow: 'tooltip',
    },
    {
      field: 'location',
      title: '登录地点',
      width: 150,
      showOverflow: 'tooltip',
    },
    {
      field: 'os',
      title: '操作系统',
      width: 120,
    },
    {
      field: 'browser',
      title: '浏览器',
      width: 110,
    },
    {
      field: 'status',
      title: '登录状态',
      width: 100,
      align: 'center',
      slots: { default: 'status' },
    },
    {
      field: 'message',
      title: '描述',
      minWidth: 200,
      slots: { default: 'message' },
    },
    {
      field: 'loginTime',
      title: '访问时间',
      minWidth: 170,
      sortable: true,
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
        placeholder: '请输入登录地址',
      },
      fieldName: 'ip',
      label: '登录地址',
    },
    {
      component: 'Input',
      componentProps: {
        allowClear: true,
        placeholder: '请输入用户名称',
      },
      fieldName: 'userName',
      label: '用户名称',
    },
    {
      component: 'Select',
      componentProps: {
        allowClear: true,
        options: LOGIN_STATUS_OPTIONS,
        placeholder: '登录状态',
      },
      fieldName: 'status',
      label: '状态',
    },
    {
      component: 'RangePicker',
      fieldName: 'loginTimeRange',
      label: '登录时间',
      componentProps: {
        valueFormat: 'YYYY-MM-DD',
      },
    },
  ];
}
