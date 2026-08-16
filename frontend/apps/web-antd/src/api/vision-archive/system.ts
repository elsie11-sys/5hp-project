/**
 * 视力档案 - 系统管理 API
 *
 * 约定：所有路径均相对于 baseURL（即 VITE_GLOB_API_URL = /api）
 * 后端真实路由：Controller [Route("api/user")] + Action 上的子路径
 */
import { requestClient } from '#/api/request';

// =====================================================================
// 类型定义
// =====================================================================

/** 后端真实返回的 UserDto（C# 那边 PascalCase，JSON 序列化后还是 PascalCase） */
export interface BackendUserDto {
  id: number;
  username: string;
  realName: string;
  gender: number;       // 0:未知 1:男 2:女
  roleId: number;
  orgId: number;
  phoneNumber: string;
  email: string;
  status: number;       // 1:启用 0:禁用
  password?: string;
  createdAt?: string;
}

/** 分页结果 */
export interface PagedResult<T> {
  items: T[];
  total: number;
}

/** 统一响应包装 */
export interface ApiResult<T = unknown> {
  code: number;
  data: T;
  message: string;
}

/** 批量导入结果 */
export interface ImportResult {
  totalRows: number;
  successCount: number;
  failedCount: number;
  errors: Array<{ row: number; message: string }>;
}

// =====================================================================
// 前端表单 -> 后端 DTO 的映射
// =====================================================================

/** 前端表单的 User（与 useUserManage.ts 保持一致） */
export interface FrontendUserForm {
  id?: number | string;
  username: string;
  name: string;        // -> realName
  gender: '' | 'male' | 'female';  // -> 1 / 2 / 0
  role: string;        // 中文名 -> roleId（见 ROLE_NAME_TO_ID）
  org: string;         // 中文名 -> orgId（见 ORG_NAME_TO_ID）
  phone: string;       // -> phoneNumber
  email: string;
  password?: string;
  status: 'active' | 'inactive';   // -> 1 / 0
}

/**
 * 临时静态映射：等后端有 sys_role / sys_org 表后改为下拉接口拉取
 * 这里给每个角色/机构编个 ID，让数据能跑通端到端
 */
const ROLE_NAME_TO_ID: Record<string, number> = {
  '国家管理员': 1,
  '省级管理员': 2,
  '市级管理员': 3,
  '县级管理员': 4,
  '学校管理员': 5,
  '校医':       6,
  '班主任':     7,
};

const ORG_NAME_TO_ID: Record<string, number> = {};

const GENDER_TO_ID: Record<string, number> = {
  '':       0,
  'male':   1,
  'female': 2,
};

const GENDER_ID_TO_NAME: Record<number, '' | 'male' | 'female'> = {
  0: '',
  1: 'male',
  2: 'female',
};

const STATUS_TO_ID: Record<string, number> = {
  'active':   1,
  'inactive': 0,
};

const STATUS_ID_TO_TEXT: Record<number, { text: string; cls: string }> = {
  1: { text: '启用', cls: 'status-active' },
  0: { text: '禁用', cls: 'status-inactive' },
};

const ROLE_ID_TO_NAME: Record<number, string> = Object.fromEntries(
  Object.entries(ROLE_NAME_TO_ID).map(([k, v]) => [v, k]),
);
const ORG_ID_TO_NAME: Record<number, string> = {};

/** 用实际组织数据初始化/更新 ID<->名称 映射 */
export function setOrgMapping(orgTree: Array<{ id: number; name: string; children?: any[] }>) {
  Object.keys(ORG_NAME_TO_ID).forEach((k) => delete ORG_NAME_TO_ID[k]);
  Object.keys(ORG_ID_TO_NAME).forEach((k) => delete ORG_ID_TO_NAME[+k]);
  const walk = (nodes: any[]) => {
    for (const n of nodes) {
      ORG_NAME_TO_ID[n.name] = n.id;
      ORG_ID_TO_NAME[n.id] = n.name;
      if (n.children?.length) walk(n.children);
    }
  };
  walk(orgTree);
}

/** 获取当前组织下拉选项 */
export function getOrgOptions() {
  return Object.keys(ORG_NAME_TO_ID).map((k) => ({ label: k, value: k }));
}

/** 前端表单 -> 后端 DTO */
export function mapFormToDto(form: FrontendUserForm): BackendUserDto {
  return {
    id: form.id ? Number(form.id) : 0,
    username: form.username,
    realName: form.name,
    gender: GENDER_TO_ID[form.gender] ?? 0,
    roleId: ROLE_NAME_TO_ID[form.role] ?? 0,
    orgId: ORG_NAME_TO_ID[form.org] ?? 0,
    phoneNumber: form.phone,
    email: form.email,
    status: STATUS_TO_ID[form.status] ?? 1,
    password: form.password,
  };
}

/** 后端 DTO -> 前端展示对象（带派生字段 role/org/statusText/roleClass） */
export function mapDtoToDisplay(dto: BackendUserDto) {
  return {
    id: dto.id,
    username: dto.username,
    name: dto.realName,
    gender: GENDER_ID_TO_NAME[dto.gender] ?? '',
    genderText: dto.gender === 1 ? '男' : dto.gender === 2 ? '女' : '未知',
    role: ROLE_ID_TO_NAME[dto.roleId] ?? `角色#${dto.roleId}`,
    org: ORG_ID_TO_NAME[dto.orgId] ?? `机构#${dto.orgId}`,
    phone: dto.phoneNumber,
    email: dto.email,
    status: dto.status === 1 ? 'active' : 'inactive',
    statusText: STATUS_ID_TO_TEXT[dto.status]?.text ?? '未知',
    statusClass: STATUS_ID_TO_TEXT[dto.status]?.cls ?? '',
    // 角色 class 简单按角色名给个颜色 tag
    roleClass: `role-tag role-${dto.roleId}`,
    // 后端没存最后登录时间，先给个占位
    lastLogin: '—',
    _raw: dto,
  };
}

// =====================================================================
// 用户管理 API（userApi 对象）
// =====================================================================

/**
 * 把后端 PagedResult 解出来（data 是分页结果）
 * 后端返回 { code: 0, data: { items, total }, message: "ok" }
 * requestClient.responseReturn = 'data' 已经把 data 字段拆出来，
 * 所以 res 直接就是 { items, total }。
 */
export const userApi = {
  /** 分页查询用户列表 */
  async getUserList(params: { page?: number; pageSize?: number; keyword?: string; orgIds?: string } = {}): Promise<PagedResult<BackendUserDto>> {
    const res = await requestClient.get<PagedResult<BackendUserDto>>('/user/paged', { params });
    return res;
  },

  /** 查询单个用户 */
  async getUser(id: number | string): Promise<BackendUserDto> {
    return requestClient.get<BackendUserDto>(`/user/${id}`);
  },

  /** 创建用户（form 为前端表单对象，内部完成 DTO 映射） */
  async createUser(form: FrontendUserForm): Promise<BackendUserDto> {
    const dto = mapFormToDto(form);
    return requestClient.post<BackendUserDto>('/user', dto);
  },

  /** 更新用户 */
  async updateUser(id: number | string, form: FrontendUserForm): Promise<BackendUserDto> {
    const dto = mapFormToDto({ ...form, id });
    return requestClient.put<BackendUserDto>(`/user/${id}`, dto);
  },

  /** 删除单个用户 */
  async deleteUser(id: number | string): Promise<void> {
    await requestClient.delete(`/user/${id}`);
  },

  /** 批量删除：调后端专用接口，一次性删 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/user/batch-delete',
      { ids: ids.map(Number) },
    );
  },

  /** 重置密码：后端会把 PasswordHash 置为 123456 */
  async resetPassword(id: number | string): Promise<void> {
    await requestClient.post(`/user/${id}/reset-password`, { newPassword: '123456' });
  },

  /** 批量导入：上传 .xlsx / .xls / .csv */
  async importUsers(formData: FormData): Promise<ImportResult> {
    return requestClient.post<ImportResult>('/user/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  /** 下载导入模板（触发浏览器下载 .xlsx） */
  downloadTemplate(): void {
    const a = document.createElement('a');
    a.href = '/api/user/template';
    a.download = `用户导入模板_${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },
};

// =====================================================================
// 角色 / 权限 / 数据权限管理
// =====================================================================

/** 角色 DTO（后端返回） */
export interface RoleDto {
  id: number;
  name: string;           // 角色名称
  roleCode: string;       // 角色编号
  code: string;           // 权限字符
  level: number;          // 等级
  status: number;         // 1:启用 0:停用
  remark?: string;        // 备注
  createdAt?: string;
  updatedAt?: string | null;
  menuIds?: number[];     // 关联菜单ID列表
  dataScope?: string;     // 数据权限范围
  userCount?: number;     // 关联用户数
}

/** 角色表单 */
export interface RoleForm {
  id?: number;
  name: string;
  roleCode: string;       // 角色编号
  code: string;           // 权限字符
  level: number;
  status: number;
  remark?: string;
}

/** 角色查询参数 */
export interface RoleQuery {
  page?: number;
  pageSize?: number;
  name?: string;
  code?: string;
  status?: number;
  startDate?: string;
  endDate?: string;
}

/** 菜单权限 DTO */
export interface MenuDto {
  id: number;
  name: string;
  code?: string;
  icon?: string;
  type: number;           // 1:目录 2:菜单 3:按钮
  parentId: number | null;
  sort: number;
  status: number;         // 1:正常 0:停用
  path?: string;
  component?: string;
  permission?: string;    // 权限标识
  isExternal: number;     // 0:否 1:是
  routeParams?: string;   // 路由 query 参数（JSON 字符串）
  isKeepAlive: number;    // 0:不缓存 1:缓存
  isVisible: number;      // 0:隐藏 1:显示
  remark?: string;
  createdAt?: string;
  updatedAt?: string | null;
  children?: MenuDto[];
}

/** 菜单表单（用于新增/编辑提交） */
export interface MenuForm {
  id?: number;
  name: string;
  code?: string;
  icon?: string;
  type: number;           // 1:目录 2:菜单 3:按钮
  parentId?: number | null;
  sort: number;
  status: number;         // 1:正常 0:停用
  path?: string;
  component?: string;
  permission?: string;
  isExternal?: number;    // 0:否 1:是
  routeParams?: string;
  isKeepAlive?: number;   // 0:不缓存 1:缓存
  isVisible?: number;     // 0:隐藏 1:显示
  remark?: string;
}

/** 菜单查询参数 */
export interface MenuQuery {
  name?: string;
  status?: number;
  type?: number;
}

/** 数据权限范围选项 */
export const DATA_SCOPE_OPTIONS = [
  { label: '全部数据权限', value: 'ALL' },
  { label: '自定义数据权限', value: 'CUSTOM' },
  { label: '本部门数据权限', value: 'DEPT' },
  { label: '本部门及以下数据权限', value: 'DEPT_AND_BELOW' },
  { label: '仅本人数据权限', value: 'SELF' },
];

/** 角色 API */
export const roleApi = {
  /** 分页查询角色列表 */
  async getPagedList(query: RoleQuery = {}): Promise<{ items: RoleDto[]; total: number }> {
    return requestClient.get<{ items: RoleDto[]; total: number }>('/role/paged', { params: query });
  },

  /** 获取所有角色（用于下拉选择） */
  async getAll(): Promise<RoleDto[]> {
    return requestClient.get<RoleDto[]>('/role/all');
  },

  /** 按 ID 查询单个角色 */
  async getById(id: number | string): Promise<RoleDto> {
    return requestClient.get<RoleDto>(`/role/${id}`);
  },

  /** 新建角色 */
  async create(data: RoleForm): Promise<RoleDto> {
    return requestClient.post<RoleDto>('/role', data);
  },

  /** 更新角色 */
  async update(id: number | string, data: RoleForm): Promise<RoleDto> {
    return requestClient.put<RoleDto>(`/role/${id}`, data);
  },

  /** 删除角色 */
  async delete(id: number | string): Promise<void> {
    await requestClient.delete(`/role/${id}`);
  },

  /** 批量删除角色 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/role/batch-delete',
      { ids: ids.map(Number) },
    );
  },

  /** 导出角色数据 */
  export(query: RoleQuery = {}): void {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== undefined && v !== null && v !== '') params.append(k, String(v));
    });
    const qs = params.toString();
    const a = document.createElement('a');
    a.href = `/api/role/export${qs ? '?' + qs : ''}`;
    a.download = `角色导出_${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },

  /** 分配菜单权限 */
  async assignMenuPermission(roleId: number | string, menuIds: number[]): Promise<void> {
    return requestClient.post<void>(`/role/${roleId}/menu-permission`, { menuIds });
  },

  /** 获取角色已分配的菜单权限 */
  async getMenuPermission(roleId: number | string): Promise<number[]> {
    return requestClient.get<number[]>(`/role/${roleId}/menu-permission`);
  },

  /** 分配数据权限 */
  async assignDataPermission(roleId: number | string, data: {
    dataScope: string;
    deptIds?: number[];
  }): Promise<void> {
    return requestClient.post<void>(`/role/${roleId}/data-permission`, data);
  },

  /** 获取角色数据权限 */
  async getDataPermission(roleId: number | string): Promise<{ dataScope: string; deptIds: number[] }> {
    return requestClient.get<{ dataScope: string; deptIds: number[] }>(`/role/${roleId}/data-permission`);
  },
};

/** 菜单权限 API */
export const menuApi = {
  /** 获取菜单树 */
  async getTree(): Promise<MenuDto[]> {
    return requestClient.get<MenuDto[]>('/menu/tree');
  },

  /** 获取所有菜单（扁平列表） */
  async getAll(): Promise<MenuDto[]> {
    return requestClient.get<MenuDto[]>('/menu/all');
  },

  /** 按 ID 查询单个菜单 */
  async getById(id: number | string): Promise<MenuDto> {
    return requestClient.get<MenuDto>(`/menu/${id}`);
  },

  /** 新建菜单 */
  async create(data: MenuForm): Promise<MenuDto> {
    return requestClient.post<MenuDto>('/menu', data);
  },

  /** 更新菜单 */
  async update(id: number | string, data: MenuForm): Promise<MenuDto> {
    return requestClient.put<MenuDto>(`/menu/${id}`, data);
  },

  /** 删除菜单 */
  async delete(id: number | string): Promise<void> {
    await requestClient.delete(`/menu/${id}`);
  },

  /** 批量删除菜单 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/menu/batch-delete',
      { ids: ids.map(Number) },
    );
  },
};

/** 状态文本映射 */
export const ROLE_STATUS_MAP: Record<number, { text: string; color: string }> = {
  1: { text: '启用', color: '#52c41a' },
  0: { text: '停用', color: '#ff4d4f' },
};

/** 角色等级选项 */
export const ROLE_LEVEL_OPTIONS = [
  { label: '1', value: 1 },
  { label: '2', value: 2 },
  { label: '3', value: 3 },
  { label: '4', value: 4 },
  { label: '5', value: 5 },
];

/** 角色下拉数据（保留兼容） */
export const ROLE_OPTIONS = Object.keys(ROLE_NAME_TO_ID).map((k) => ({ label: k, value: k }));
export { getOrgOptions as ORG_OPTIONS };

// 保留旧函数名兼容
export const getRoleList = () => roleApi.getPagedList();
export const createRole = (data: any) => roleApi.create(data);
export const updateRole = (id: string, data: any) => roleApi.update(id, data);
export const deleteRole = (id: string) => roleApi.delete(id);
export const getPermissionTree = () => menuApi.getTree();

// 组织管理（占位）
export function getOrgTree() {
  return requestClient.get('/system/org/tree');
}

// =====================================================================
// 字典管理（dictApi：对接后端 SysDictController）
// =====================================================================

/** 字典项 DTO */
export interface DictItemDto {
  id?: number | string;
  dictType: string;
  itemCode?: string;
  itemLabel: string;
  itemValue: string;
  sortOrder: number;
  status: number;
  remark?: string;
  isNew?: boolean;
  isDeleted?: boolean;
}

/** 字典 DTO */
export interface DictDto {
  id?: number;
  dictName: string;
  dictType: string;
  status: number;        // 1=正常 0=停用
  ownerType?: 'SYSTEM' | 'USER';  // 归属类型：SYSTEM=系统内置，USER=用户自定义
  remark?: string;
  createdAt?: string;
  updatedAt?: string | null;
  dictItems?: DictItemDto[];
}

/** 字典分页查询 */
export interface DictQuery {
  page?: number;
  pageSize?: number;
  dictName?: string;
  dictType?: string;
  status?: number;
  ownerType?: 'SYSTEM' | 'USER';
  startDate?: string;    // ISO 字符串
  endDate?: string;
}

export const dictApi = {
  /** 分页查询 */
  async getPaged(query: DictQuery = {}): Promise<{ items: DictDto[]; total: number }> {
    return requestClient.get<{ items: DictDto[]; total: number }>('/dict/paged', { params: query });
  },

  /** 全量（用于下拉/缓存） */
  async getAll(): Promise<DictDto[]> {
    return requestClient.get<DictDto[]>('/dict/all');
  },

  /** 按 ID 查单个 */
  async getById(id: number | string): Promise<DictDto> {
    return requestClient.get<DictDto>(`/dict/${id}`);
  },

  /** 按类型查单个 */
  async getByType(type: string): Promise<DictDto> {
    return requestClient.get<DictDto>(`/dict/type/${type}`);
  },

  /** 新建 */
  async create(data: DictDto): Promise<DictDto> {
    return requestClient.post<DictDto>('/dict', data);
  },

  /** 更新 */
  async update(id: number | string, data: DictDto): Promise<DictDto> {
    return requestClient.put<DictDto>(`/dict/${id}`, data);
  },

  /** 删除 */
  async delete(id: number | string): Promise<void> {
    await requestClient.delete(`/dict/${id}`);
  },

  /** 批量删除 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/dict/batch-delete',
      { ids: ids.map(Number) },
    );
  },

  /** 检查 DictType 是否被占用（编辑时实时反馈） */
  async checkType(type: string, excludeId?: number): Promise<{ exists: boolean }> {
    return requestClient.get<{ exists: boolean }>('/dict/check-type', { params: { type, excludeId } });
  },

  /** 导出（按当前查询条件）—— 浏览器直接下载 */
  export(query: DictQuery = {}): void {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== undefined && v !== null && v !== '') params.append(k, String(v));
    });
    const qs = params.toString();
    const a = document.createElement('a');
    a.href = `/api/dict/export${qs ? '?' + qs : ''}`;
    a.download = `字典导出_${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },

  /** 刷新缓存（后端占位端点） */
  async refreshCache(): Promise<{ message: string }> {
    return requestClient.post<{ message: string }>('/dict/refresh-cache');
  },
};

// 字典管理（占位）
export function getDictionaryList() {
  return requestClient.get('/system/dict/list');
}

// 日志（占位）
export function getOperationLogList() {
  return requestClient.get('/system/log/operation');
}
export function getLoginLogList() {
  return requestClient.get('/system/log/login');
}

// =====================================================================
// 组织/部门管理（orgApi）
// =====================================================================

/** 组织 DTO（后端返回） */
export interface OrgDto {
  id: number;
  name: string;
  code?: string;
  level?: string;       // 级别：国家级/省级/市级/区县级/学校级
  parentId?: number | null;
  sort: number;         // 排序
  status: number;       // 1:正常 0:停用
  remark?: string;
  leader?: string;      // 负责人
  phone?: string;       // 联系电话
  email?: string;       // 邮箱
  createdAt?: string;
  updatedAt?: string | null;
  children?: OrgDto[];
}

/** 组织表单 */
export interface OrgForm {
  id?: number;
  parentId?: number | null;
  name: string;
  sort: number;
  level: string;
  leader?: string;
  phone?: string;
  email?: string;
  status: number;
}

/** 组织查询参数 */
export interface OrgQuery {
  page?: number;
  pageSize?: number;
  name?: string;
  status?: number;
}

/** 组织 API */
export const orgApi = {
  /** 分页查询组织列表 */
  async getPagedList(query: OrgQuery = {}): Promise<{ items: OrgDto[]; total: number }> {
    return requestClient.get<{ items: OrgDto[]; total: number }>('/org/paged', { params: query });
  },

  /** 获取组织树 */
  async getTree(): Promise<OrgDto[]> {
    return requestClient.get<OrgDto[]>('/org/tree');
  },

  /** 获取所有组织（扁平列表） */
  async getAll(): Promise<OrgDto[]> {
    return requestClient.get<OrgDto[]>('/org/all');
  },

  /** 按 ID 查询单个组织 */
  async getById(id: number | string): Promise<OrgDto> {
    return requestClient.get<OrgDto>(`/org/${id}`);
  },

  /** 新建组织 */
  async create(data: OrgForm): Promise<OrgDto> {
    return requestClient.post<OrgDto>('/org', data);
  },

  /** 更新组织 */
  async update(id: number | string, data: OrgForm): Promise<OrgDto> {
    return requestClient.put<OrgDto>(`/org/${id}`, data);
  },

  /** 删除组织 */
  async delete(id: number | string): Promise<{ code: number; message: string }> {
    return requestClient.delete<{ code: number; message: string }>(`/org/${id}`);
  },

  /** 批量删除组织 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/org/batch-delete',
      { ids: ids.map(Number) },
    );
  },
};

/** 状态文本映射 */
export const ORG_STATUS_MAP: Record<number, { text: string; color: string }> = {
  1: { text: '正常', color: '#52c41a' },
  0: { text: '停用', color: '#ff4d4f' },
};

/** 组织级别选项 */
export const ORG_LEVEL_OPTIONS = [
  { label: '国家级', value: '国家级' },
  { label: '省级', value: '省级' },
  { label: '市级', value: '市级' },
  { label: '区县级', value: '区县级' },
  { label: '学校级', value: '学校级' },
];
