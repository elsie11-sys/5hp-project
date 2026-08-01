import type { Recordable } from '@vben/types';

import { requestClient } from '#/api/request';

export namespace SystemMenuApi {
  /** 菜单类型集合 */
  export const MenuTypes = [
    { label: '目录', value: 1 },
    { label: '菜单', value: 2 },
    { label: '按钮', value: 3 },
  ] as const;

  /** 菜单状态 */
  export const MenuStatus = [
    { label: '正常', value: 1 },
    { label: '停用', value: 0 },
  ] as const;

  /** 系统菜单 */
  export interface SystemMenu {
    [key: string]: any;
    /** 菜单ID */
    id: string | number;
    /** 菜单名称 */
    name: string;
    /** 权限字符 */
    code?: string;
    /** 图标 */
    icon?: string;
    /** 类型：1=目录 2=菜单 3=按钮 */
    type: number;
    /** 父级ID */
    parentId?: number | null;
    /** 排序 */
    sort: number;
    /** 状态：1=正常 0=停用 */
    status: number;
    /** 路由路径 */
    path?: string;
    /** 组件路径 */
    component?: string;
    /** 权限标识 */
    permission?: string;
    /** 是否外链：0=否 1=是 */
    isExternal?: number;
    /** 路由参数 */
    routeParams?: string;
    /** 是否缓存：0=不缓存 1=缓存 */
    isKeepAlive?: number;
    /** 显示状态：0=隐藏 1=显示 */
    isVisible?: number;
    /** 备注 */
    remark?: string;
    /** 创建时间 */
    createdAt?: string;
    /** 更新时间 */
    updatedAt?: string;
    /** 子菜单 */
    children?: SystemMenu[];
  }

  /** 菜单查询参数 */
  export interface MenuQuery {
    name?: string;
    status?: number;
  }
}

/**
 * 获取菜单列表
 */
async function getMenuList(params?: SystemMenuApi.MenuQuery) {
  return requestClient.get<Array<SystemMenuApi.SystemMenu>>(
    '/menu/list',
    { params },
  );
}

/**
 * 获取菜单树
 */
async function getMenuTree(params?: SystemMenuApi.MenuQuery) {
  return requestClient.get<Array<SystemMenuApi.SystemMenu>>(
    '/menu/tree',
    { params },
  );
}

/**
 * 获取所有菜单（扁平列表）
 */
async function getAllMenus() {
  return requestClient.get<Array<SystemMenuApi.SystemMenu>>('/menu/all');
}

/**
 * 根据ID获取菜单
 */
async function getMenuById(id: number) {
  return requestClient.get<SystemMenuApi.SystemMenu>(`/menu/${id}`);
}

/**
 * 创建菜单
 */
async function createMenu(
  data: Omit<SystemMenuApi.SystemMenu, 'id' | 'children'>,
) {
  return requestClient.post('/menu', data);
}

/**
 * 更新菜单
 */
async function updateMenu(
  id: number,
  data: Omit<SystemMenuApi.SystemMenu, 'id' | 'children'>,
) {
  return requestClient.put(`/menu/${id}`, data);
}

/**
 * 删除菜单
 */
async function deleteMenu(id: number) {
  return requestClient.delete(`/menu/${id}`);
}

/**
 * 批量删除菜单
 */
async function batchDeleteMenus(ids: number[]) {
  return requestClient.post('/menu/batch-delete', { ids });
}

export {
  batchDeleteMenus,
  createMenu,
  deleteMenu,
  getAllMenus,
  getMenuById,
  getMenuList,
  getMenuTree,
  updateMenu,
};
