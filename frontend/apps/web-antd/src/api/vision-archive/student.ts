/**
 * 视力档案 - 学生档案管理 API
 *
 * 约定：所有路径均相对于 baseURL（即 VITE_GLOB_API_URL = /api）
 * 后端真实路由：Controller [Route("api/student")] + Action 上的子路径
 *
 * 后端 StudentController 已经统一了响应包装（{code, data, message}），
 * requestClient.responseReturn = 'data' 会自动把 data 字段拆出来。
 */
import { requestClient } from '#/api/request';

// =====================================================================
// 类型定义（与后端 StudentDto 一一对应）
// =====================================================================

/** 后端真实返回的 StudentDto（PascalCase 经 JSON camelCase 序列化后） */
export interface BackendStudentDto {
  id: number;
  studentNo: string;
  avatar: string;
  gradeYear: string;
  school: string;
  className: string;
  name: string;
  gender: string;            // 男 / 女
  nation: string;            // 民族
  birthday: string | null;   // ISO 日期字符串 yyyy-MM-dd
  idCard: string;
  nativePlace: string;
  address: string;
  parentPhone: string;
  parentContact: string;
  allergyHistory: string;
  fatherMyopia: string;
  motherMyopia: string;
  dataSource: string;
  intervention: boolean;
  createdAt?: string;
  updatedAt?: string;
}

/** 分页结果 */
export interface PagedResult<T> {
  items: T[];
  total: number;
}

/** 通用响应包装（已由 requestClient 自动拆出 data，但部分接口仍要看到原包装） */
export interface ApiResult<T = unknown> {
  code: number;
  data: T;
  message: string;
}

/** 学生档案查询参数（与后端 StudentQuery 对应） */
export interface StudentQuery {
  page?: number;
  pageSize?: number;
  school?: string;
  className?: string;
  name?: string;
  studentNo?: string;
  intervention?: boolean;
  keyword?: string;
}

/** 前端表单对象（与后端 StudentDto 字段一致，方便直接发请求） */
export interface StudentForm {
  id?: number;
  studentNo: string;
  avatar: string;
  gradeYear: string;
  school: string;
  className: string;
  name: string;
  gender: string;
  nation: string;
  birthday: string | null;
  idCard: string;
  nativePlace: string;
  address: string;
  parentPhone: string;
  parentContact: string;
  allergyHistory: string;
  fatherMyopia: string;
  motherMyopia: string;
  dataSource: string;
  intervention: boolean;
}

// =====================================================================
// 前端表单 -> 后端 DTO 的映射
// =====================================================================

/** 表单 -> DTO（去除 undefined，确保类型干净） */
export function mapFormToDto(form: StudentForm): BackendStudentDto {
  return {
    id: form.id ?? 0,
    studentNo: form.studentNo,
    avatar: form.avatar,
    gradeYear: form.gradeYear,
    school: form.school,
    className: form.className,
    name: form.name,
    gender: form.gender,
    nation: form.nation,
    birthday: form.birthday,
    idCard: form.idCard,
    nativePlace: form.nativePlace,
    address: form.address,
    parentPhone: form.parentPhone,
    parentContact: form.parentContact,
    allergyHistory: form.allergyHistory,
    fatherMyopia: form.fatherMyopia,
    motherMyopia: form.motherMyopia,
    dataSource: form.dataSource,
    intervention: form.intervention,
  };
}

/** 后端 DTO -> 前端表单（直接复用同一个对象即可） */
export function mapDtoToForm(dto: BackendStudentDto): StudentForm {
  return {
    id: dto.id,
    studentNo: dto.studentNo,
    avatar: dto.avatar,
    gradeYear: dto.gradeYear,
    school: dto.school,
    className: dto.className,
    name: dto.name,
    gender: dto.gender,
    nation: dto.nation,
    birthday: dto.birthday,
    idCard: dto.idCard,
    nativePlace: dto.nativePlace,
    address: dto.address,
    parentPhone: dto.parentPhone,
    parentContact: dto.parentContact,
    allergyHistory: dto.allergyHistory,
    fatherMyopia: dto.fatherMyopia,
    motherMyopia: dto.motherMyopia,
    dataSource: dto.dataSource,
    intervention: dto.intervention,
  };
}

// =====================================================================
// 学生档案管理 API（studentApi）
// =====================================================================

export const studentApi = {
  /**
   * 分页查询学生档案
   * GET /api/student/paged?page=&pageSize=&school=&className=&name=&intervention=
   */
  async getPagedList(
    query: StudentQuery = {},
  ): Promise<PagedResult<BackendStudentDto>> {
    // 后端 intervention 是 bool，传 true/false；前端用 '' 表达"全部"，所以这里做转换
    const params: Record<string, any> = { ...query };
    if (params.intervention === undefined) {
      delete params.intervention;
    }
    return requestClient.get<PagedResult<BackendStudentDto>>(
      '/student/paged',
      { params },
    );
  },

  /** 获取所有学生档案（用于下拉） */
  async getAll(): Promise<BackendStudentDto[]> {
    return requestClient.get<BackendStudentDto[]>('/student/all');
  },

  /** 按 ID 获取 */
  async getById(id: number | string): Promise<BackendStudentDto> {
    return requestClient.get<BackendStudentDto>(`/student/${id}`);
  },

  /** 按学号获取 */
  async getByNo(studentNo: string): Promise<BackendStudentDto> {
    return requestClient.get<BackendStudentDto>(`/student/by-no/${encodeURIComponent(studentNo)}`);
  },

  /** 创建学生档案（form 为前端表单对象） */
  async create(form: StudentForm): Promise<BackendStudentDto> {
    const dto = mapFormToDto(form);
    return requestClient.post<BackendStudentDto>('/student', dto);
  },

  /** 更新学生档案 */
  async update(id: number | string, form: StudentForm): Promise<BackendStudentDto> {
    const dto = mapFormToDto({ ...form, id: Number(id) });
    return requestClient.put<BackendStudentDto>(`/student/${id}`, dto);
  },

  /** 删除单个学生 */
  async delete(id: number | string): Promise<void> {
    await requestClient.delete(`/student/${id}`);
  },

  /** 批量删除学生 */
  async batchDelete(ids: Array<number | string>): Promise<{ deletedCount: number; message: string }> {
    return requestClient.post<{ deletedCount: number; message: string }>(
      '/student/batch-delete',
      { ids: ids.map(Number) },
    );
  },

  /** 切换是否干预状态 */
  async toggleIntervention(
    id: number | string,
    intervention: boolean,
  ): Promise<{ message: string; intervention: boolean }> {
    return requestClient.post<{ message: string; intervention: boolean }>(
      `/student/${id}/toggle-intervention`,
      { intervention },
    );
  },

  /** 检查学号是否已被占用（编辑时实时反馈） */
  async checkStudentNo(
    studentNo: string,
    excludeId?: number,
  ): Promise<{ exists: boolean }> {
    return requestClient.get<{ exists: boolean }>('/student/check-no', {
      params: { studentNo, excludeId },
    });
  },
};

// =====================================================================
// 头像上传
// =====================================================================

/** 后端返回的上传结果 */
export interface UploadAvatarResult {
  url: string;        // 相对路径，例如 /uploads/avatars/20260818_xxx.png
  fileName: string;
  size: number;
}

/**
 * 上传头像到后端，返回可直接访问的 URL
 * POST /api/upload/avatar  multipart/form-data  field = "file"
 */
export function uploadAvatar(file: File | Blob, fileName?: string): Promise<UploadAvatarResult> {
  const formData = new FormData();
  // 给 Blob 一个文件名（有些后端会校验 FileName）
  const name = fileName || (file instanceof File ? file.name : 'avatar.png');
  formData.append('file', file, name);
  return requestClient.post<UploadAvatarResult>('/upload/avatar', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
