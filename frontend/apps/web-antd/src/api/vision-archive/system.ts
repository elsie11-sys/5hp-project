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
  code: string;           // 角色编号（业务标识，唯一）
  permission?: string;    // 权限字符（控制器/注解中使用的权限标识）
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
  code: string;           // 角色编号（业务标识，唯一）
  permission?: string;    // 权限字符
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

/** 数据权限范围选项：value 与 sys_dict 中 DictType=role_based_data_permissions 的 ItemValue 一一对应 */
export const DATA_SCOPE_OPTIONS = [
  { label: '全部数据权限', value: '1' },
  { label: '自定义数据权限', value: '2' },
  { label: '本部门数据权限', value: '3' },
  { label: '本部门及以下数据权限', value: '4' },
  { label: '仅本人数据权限', value: '5' },
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

/** 角色等级选项（数值越小权限越高，1=国家级 最高，5=学校级 最低） */
export const ROLE_LEVEL_OPTIONS = [
  { label: '1 - 国家级', value: 1 },
  { label: '2 - 省级', value: 2 },
  { label: '3 - 市级', value: 3 },
  { label: '4 - 区县级', value: 4 },
  { label: '5 - 学校级', value: 5 },
];

/** 角色等级数值到名称的映射（用于表格列展示） */
export const ROLE_LEVEL_MAP: Record<number, string> = {
  1: '国家级',
  2: '省级',
  3: '市级',
  4: '区县级',
  5: '学校级',
};

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

// =====================================================================
// 操作日志（operationLogApi）
// =====================================================================

/** 操作日志查询参数 */
export interface OperationLogQuery {
  page?: number;
  pageSize?: number;
  /** 操作人（模糊） */
  operator?: string;
  /** 操作类型：login/create/update/delete/export/import/query/other */
  type?: string;
  /** 操作模块（模糊） */
  module?: string;
  /** 客户端 IP（模糊） */
  ip?: string;
  /** 1=成功 0=失败 */
  status?: 0 | 1;
  /** 开始日期 YYYY-MM-DD */
  startDate?: string;
  /** 结束日期 YYYY-MM-DD */
  endDate?: string;
}

/** 操作日志 DTO（前端展示用，结构与视图层一致） */
export interface OperationLogDto {
  id: number;
  operator: string;
  type: string;
  module: string;
  content: string;
  ip: string;
  status: 0 | 1;
  costMs: number;
  createdAt: string;
  userAgent?: string;
  location?: string;
  /** 请求地址，如 /system/role/changeStatus */
  requestUrl?: string;
  /** 请求方式：GET / POST / PUT / DELETE */
  requestMethod?: 'GET' | 'POST' | 'PUT' | 'DELETE' | string;
  /** 操作方法（Java 全限定方法名 + 括号） */
  method?: string;
  /** 请求参数（JSON 字符串） */
  requestParams?: string;
  /** 返回参数（JSON 字符串） */
  responseParams?: string;
  /** 错误消息（status=0 时） */
  errorMsg?: string;
}

// ---------- 本地 mock（后端未就绪时使用，命中规则：endpoint 404/网络异常） ----------

/** 简易可复现的伪随机（保证 mock 数据在每次刷新后保持稳定） */
function makeRng(seed: number) {
  let s = seed >>> 0;
  return () => {
    s = (s * 1664525 + 1013904223) >>> 0;
    return s / 0xffffffff;
  };
}

const MOCK_TYPES = ['login', 'create', 'update', 'delete', 'export', 'import', 'query', 'other'];
const MOCK_MODULES = ['系统管理', '学生档案', '数据采集', '统计分析', '用户管理', '组织架构', '字典管理', '权限管理'];
const MOCK_USERS = ['admin', '张伟', '李明', '王芳', '陈晓', '刘强', '赵敏', '周婷', '吴磊', 'sjwvision'];
const MOCK_IPS = ['112.7.103.208', '218.201.142.114', '192.168.1.100', '192.168.1.101', '192.168.1.102', '10.0.0.5', '172.16.20.18'];
const MOCK_LOCATIONS = ['北京 北京市', '上海 上海市', '广东 深圳市', '浙江 杭州市', '四川 成都市', '湖北 武汉市', '—'];

function pickOne<T>(rng: () => number, list: readonly T[]): T {
  return list[Math.floor(rng() * list.length)] as T;
}

function buildMockList(count: number): OperationLogDto[] {
  const rng = makeRng(20260818);
  const baseTs = Date.parse('2026-08-18T22:00:00+08:00');
  const arr: OperationLogDto[] = [];
  for (let i = 0; i < count; i++) {
    const type = pickOne(rng, MOCK_TYPES);
    const module = pickOne(rng, MOCK_MODULES);
    const operator = pickOne(rng, MOCK_USERS);
    const ip = pickOne(rng, MOCK_IPS);
    const status: 0 | 1 = rng() < 0.92 ? 1 : 0;
    const costMs = Math.floor(7 + rng() * 993);
    // 越早的越旧：id 越大时间越近
    const offsetMin = i * 17 + Math.floor(rng() * 30);
    const createdAt = new Date(baseTs - offsetMin * 60_000).toISOString().replace('T', ' ').slice(0, 19);
    const detail = buildMockDetail(type, module, operator, status);
    arr.push({
      id: count - i,
      operator,
      type,
      module,
      content: buildMockContent(type, module, operator),
      ip,
      status,
      costMs,
      createdAt,
      userAgent: 'Chrome 14 / Windows 10',
      location: pickOne(rng, MOCK_LOCATIONS),
      requestUrl: detail.requestUrl,
      requestMethod: detail.requestMethod,
      method: detail.method,
      requestParams: detail.requestParams,
      responseParams: detail.responseParams,
      errorMsg: status === 0 ? 'java.lang.RuntimeException: 业务校验失败' : undefined,
    });
  }
  return arr;
}

/**
 * 根据 type 生成更"真实"的详情（请求地址 / 请求方式 / 方法签名 / 请求&返回参数）。
 * 这里只 mock 几种典型组合，覆盖设计稿里截图的样子。
 */
function buildMockDetail(
  type: string,
  module: string,
  operator: string,
  status: 0 | 1,
): {
  requestUrl: string;
  requestMethod: 'GET' | 'POST' | 'PUT' | 'DELETE';
  method: string;
  requestParams: string;
  responseParams: string;
} {
  const controller = 'com.ruoyi.web.controller.system';
  const actionMap: Record<string, { url: string; method: 'GET' | 'POST' | 'PUT' | 'DELETE'; className: string; action: string }> = {
    login: { url: '/system/login', method: 'POST', className: 'SysLoginController', action: 'login' },
    logout: { url: '/system/logout', method: 'POST', className: 'SysLoginController', action: 'logout' },
    create: { url: `/${pinyinOf(module)}/add`, method: 'POST', className: 'SysController', action: 'add' },
    update: { url: `/${pinyinOf(module)}/edit`, method: 'PUT', className: 'SysController', action: 'edit' },
    delete: { url: `/${pinyinOf(module)}/remove/${1 + Math.floor(Math.random() * 99)}`, method: 'DELETE', className: 'SysController', action: 'remove' },
    export: { url: `/${pinyinOf(module)}/export`, method: 'POST', className: 'SysController', action: 'export' },
    import: { url: `/${pinyinOf(module)}/importData`, method: 'POST', className: 'SysController', action: 'importData' },
    query: { url: `/${pinyinOf(module)}/list`, method: 'GET', className: 'SysController', action: 'list' },
    other: { url: `/${pinyinOf(module)}/other`, method: 'GET', className: 'SysController', action: 'other' },
  };
  const m = actionMap[type] ?? actionMap.other!;
  const requestParams = mockRequestParams(type, m.action, operator);
  const responseParams = status === 1
    ? '{"msg":"操作成功","code":200}'
    : '{"msg":"操作失败","code":500}';
  return {
    requestUrl: m.url,
    requestMethod: m.method,
    method: `${controller}.${m.className}.${m.action}()`,
    requestParams,
    responseParams,
  };
}

/** 模块名 -> 拼音路径的简单映射（mock 够用就行） */
function pinyinOf(module: string): string {
  const map: Record<string, string> = {
    系统管理: 'system',
    学生档案: 'student',
    数据采集: 'collect',
    统计分析: 'stat',
    用户管理: 'user',
    组织架构: 'org',
    字典管理: 'dict',
    权限管理: 'role',
  };
  return map[module] ?? 'system';
}

function mockRequestParams(type: string, action: string, operator: string): string {
  switch (type) {
    case 'login':
      return JSON.stringify({ username: operator, password: '******', code: '****', uuid: 'xxxx-xxxx' });
    case 'logout':
      return '{}';
    case 'create':
      return JSON.stringify(
        { name: '新增_' + operator, remark: 'mock', status: 1, createBy: operator },
        null,
        0,
      );
    case 'update': {
      const obj: Record<string, unknown> = {
        admin: false,
        deptCheckStrictly: false,
        flag: false,
        menuCheckStrictly: false,
        params: {},
        roleId: 2,
        status: '0',
        updateBy: operator,
      };
      return JSON.stringify(obj, null, 0);
    }
    case 'delete':
      return JSON.stringify({ ids: '1,2,3' });
    case 'export':
      return JSON.stringify({ pageNum: 1, pageSize: 10, keyword: '' });
    case 'import':
      return '{}';
    case 'query':
      return JSON.stringify({ pageNum: 1, pageSize: 10 });
    default:
      return `{} /* ${action} */`;
  }
}

function buildMockContent(type: string, module: string, operator: string): string {
  switch (type) {
    case 'login':
      return `${operator} 登录系统`;
    case 'logout':
      return `${operator} 退出系统`;
    case 'create':
      return `${operator} 在【${module}】中新增了一条记录`;
    case 'update':
      return `${operator} 在【${module}】中修改了一条记录`;
    case 'delete':
      return `${operator} 在【${module}】中删除了一条记录`;
    case 'export':
      return `${operator} 导出【${module}】数据`;
    case 'import':
      return `${operator} 导入【${module}】数据`;
    case 'query':
      return `${operator} 查询【${module}】列表`;
    default:
      return `${operator} 在【${module}】执行了其它操作`;
  }
}

// 一次性生成 650 条，对齐设计稿里的"共 650 条"
let MOCK_CACHE: OperationLogDto[] | null = null;
function getMockList(): OperationLogDto[] {
  if (!MOCK_CACHE) MOCK_CACHE = buildMockList(650);
  return MOCK_CACHE;
}

function filterMock(query: OperationLogQuery): { items: OperationLogDto[]; total: number } {
  const list = getMockList();
  const op = query.operator?.trim();
  const mod = query.module?.trim();
  const ip = query.ip?.trim();
  const items = list.filter((row) => {
    if (op && !row.operator.includes(op)) return false;
    if (query.type && row.type !== query.type) return false;
    if (mod && !row.module.includes(mod)) return false;
    if (ip && !row.ip.includes(ip)) return false;
    if (query.status != null && row.status !== query.status) return false;
    if (query.startDate && row.createdAt.slice(0, 10) < query.startDate) return false;
    if (query.endDate && row.createdAt.slice(0, 10) > query.endDate) return false;
    return true;
  });
  const page = Math.max(1, query.page ?? 1);
  const pageSize = Math.max(1, query.pageSize ?? 10);
  const start = (page - 1) * pageSize;
  return { items: items.slice(start, start + pageSize), total: items.length };
}

/** 判断是否需要 fallback 到 mock：4xx/5xx/网络异常时返回 true */
function shouldFallback(err: any): boolean {
  const status = err?.response?.status ?? err?.status;
  if (status === 404 || status === 501) return true;
  if (status >= 500) return true;
  // 网络层错误（无 status）也 fallback
  if (!status) return true;
  return false;
}

/** 日志 API：后端未实现时自动 fallback 到本地 mock，控制台提示 */
export const operationLogApi = {
  /** 分页查询操作日志 */
  async getPagedList(query: OperationLogQuery = {}): Promise<{ items: OperationLogDto[]; total: number }> {
    try {
      return await requestClient.get<{ items: OperationLogDto[]; total: number }>(
        '/system/log/operation/paged',
        { params: query },
      );
    } catch (err) {
      if (shouldFallback(err)) {
        // eslint-disable-next-line no-console
        console.info(
          '%c[operationLogApi] 后端 /system/log/operation/paged 暂未实现，已使用本地 mock 数据（650 条）。',
          'color:#faad14',
        );
        return filterMock(query);
      }
      throw err;
    }
  },

  /** 批量删除 */
  async batchDelete(ids: number[]): Promise<{ deletedCount: number }> {
    try {
      return await requestClient.post<{ deletedCount: number }>(
        '/system/log/operation/batch-delete',
        { ids },
      );
    } catch (err) {
      if (shouldFallback(err)) {
        MOCK_CACHE = (MOCK_CACHE ?? getMockList()).filter((row) => !ids.includes(row.id));
        return { deletedCount: ids.length };
      }
      throw err;
    }
  },

  /** 清空全部 */
  async clear(): Promise<void> {
    try {
      await requestClient.post('/system/log/operation/clear');
    } catch (err) {
      if (shouldFallback(err)) {
        MOCK_CACHE = [];
        return;
      }
      throw err;
    }
  },

  /** 导出（前端直接拉文件流） */
  export(query: OperationLogQuery = {}): void {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== undefined && v !== '' && v !== null) params.append(k, String(v));
    });
    const qs = params.toString();
    const a = document.createElement('a');
    a.href = `/api/system/log/operation/export${qs ? '?' + qs : ''}`;
    a.download = `操作日志_${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },
};

// 兼容旧引用，避免出现 undefined 调用
export const getOperationLogList = () => operationLogApi.getPagedList({ page: 1, pageSize: 1000 });
export const getLoginLogList = () => requestClient.get('/system/log/login');

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

// =====================================================================
// 登录日志（loginLogApi）
// =====================================================================

/** 登录日志查询参数 */
export interface LoginLogQuery {
  page?: number;
  pageSize?: number;
  /** 登录地址（IP 模糊） */
  ip?: string;
  /** 用户名称（模糊） */
  userName?: string;
  /** 登录状态：1=成功 0=失败 */
  status?: 0 | 1;
  /** 开始日期 YYYY-MM-DD */
  startDate?: string;
  /** 结束日期 YYYY-MM-DD */
  endDate?: string;
}

/** 登录日志 DTO（前端展示用） */
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

/** 登录日志创建请求（前端登录成功/失败时调用） */
export interface LoginLogCreateRequest {
  /** 用户名（必填） */
  userName: string;
  /** 登录地址（IP，可选；后端会从 HttpContext 兜底） */
  ip?: string;
  /** 登录地点 */
  location?: string;
  /** 操作系统 */
  os?: string;
  /** 浏览器 */
  browser?: string;
  /** 1=成功 0=失败 */
  status: 0 | 1;
  /** 描述（登录成功 / 失败原因） */
  message?: string;
  /** 登录时间（可选；不传时由后端填当前时间） */
  loginTime?: string;
}

// ---------- 登录日志 API：直接对接后端 /api/system/log/login/*（后端已实现，无需 mock） ----------
export const loginLogApi = {
  /**
   * 写入一条登录日志（前端登录成功/失败时调用）
   * <para>POST /api/system/log/login  body: LoginLogCreateRequest</para>
   */
  async create(
    request: LoginLogCreateRequest,
  ): Promise<{ id: number; message: string; loginTime?: string }> {
    return requestClient.post<{ id: number; message: string; loginTime?: string }>(
      '/system/log/login',
      request,
    );
  },

  /** 分页查询登录日志 */
  async getPagedList(
    query: LoginLogQuery = {},
  ): Promise<{ items: LoginLogDto[]; total: number }> {
    return requestClient.get<{ items: LoginLogDto[]; total: number }>(
      '/system/log/login/paged',
      { params: query },
    );
  },

  /** 批量删除 */
  async batchDelete(ids: number[]): Promise<{ deletedCount: number }> {
    return requestClient.post<{ deletedCount: number }>(
      '/system/log/login/batch-delete',
      { ids },
    );
  },

  /** 清空全部 */
  async clear(): Promise<void> {
    await requestClient.post('/system/log/login/clear');
  },

  /** 账户解锁（按用户名） */
  async unlock(userName: string): Promise<{ message: string }> {
    return requestClient.post<{ message: string }>(
      '/system/log/login/unlock',
      { userName },
    );
  },

  /** 导出（前端直接拉文件流） */
  export(query: LoginLogQuery = {}): void {
    const params = new URLSearchParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== undefined && v !== '' && v !== null) params.append(k, String(v));
    });
    const qs = params.toString();
    const a = document.createElement('a');
    a.href = `/api/system/log/login/export${qs ? '?' + qs : ''}`;
    a.download = `登录日志_${new Date().toISOString().slice(0, 10)}.xlsx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  },
};
