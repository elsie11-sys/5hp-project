/**
 * 视力档案 - 学生健康档案 API
 *
 * 5 个健康维度：视力 / 口腔 / 心理 / 体重 / 骨骼
 * 每个维度 4 个接口：按学生查历史 / 查最新 / 新增 / 删除
 *
 * 后端真实路由前缀：/api/health-archive
 */
import { requestClient } from '#/api/request';

// =========================================================
// 类型定义（与后端 DTO 一一对应）
// =========================================================

export interface VisionRecordDto {
  id: number;
  studentId: number;
  checkDate: string;     // yyyy-MM-dd
  leftEye: string;
  rightEye: string;
  visionLevel: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface OralRecordDto {
  id: number;
  studentId: number;
  checkDate: string;
  toothStatus: string;
  cavityCount: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface MentalRecordDto {
  id: number;
  studentId: number;
  checkDate: string;
  stressLevel: string;
  sleepQuality: string;
  moodStatus: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface WeightRecordDto {
  id: number;
  studentId: number;
  checkDate: string;
  height: string;
  weight: string;
  bmi: string;
  bmiLevel: string;
  waistCircumference: string;
  hipCircumference: string;
  whr: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface BoneRecordDto {
  id: number;
  studentId: number;
  checkDate: string;
  boneDensity: string;
  boneAge: string;
  vitaminD: string;
  calciumLevel: string;
  createdAt?: string;
  updatedAt?: string;
}

// =========================================================
// 健康档案 API
// =========================================================

export const healthArchiveApi = {
  // ---- 视力 ----
  vision: {
    /** 按学生查全部历史（按检查日期倒序） */
    async getByStudent(studentId: number | string): Promise<VisionRecordDto[]> {
      return requestClient.get<VisionRecordDto[]>(`/health-archive/vision/by-student/${studentId}`);
    },
    /** 最新一条 */
    async getLatest(studentId: number | string): Promise<VisionRecordDto> {
      return requestClient.get<VisionRecordDto>(`/health-archive/vision/latest/${studentId}`);
    },
    /** 新增一条 */
    async create(dto: Omit<VisionRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<VisionRecordDto> {
      return requestClient.post<VisionRecordDto>('/health-archive/vision', dto);
    },
    /** 删除一条 */
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/vision/${id}`);
    },
  },

  // ---- 口腔 ----
  oral: {
    async getByStudent(studentId: number | string): Promise<OralRecordDto[]> {
      return requestClient.get<OralRecordDto[]>(`/health-archive/oral/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<OralRecordDto> {
      return requestClient.get<OralRecordDto>(`/health-archive/oral/latest/${studentId}`);
    },
    async create(dto: Omit<OralRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<OralRecordDto> {
      return requestClient.post<OralRecordDto>('/health-archive/oral', dto);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/oral/${id}`);
    },
  },

  // ---- 心理 ----
  mental: {
    async getByStudent(studentId: number | string): Promise<MentalRecordDto[]> {
      return requestClient.get<MentalRecordDto[]>(`/health-archive/mental/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<MentalRecordDto> {
      return requestClient.get<MentalRecordDto>(`/health-archive/mental/latest/${studentId}`);
    },
    async create(dto: Omit<MentalRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<MentalRecordDto> {
      return requestClient.post<MentalRecordDto>('/health-archive/mental', dto);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/mental/${id}`);
    },
  },

  // ---- 体重 ----
  weight: {
    async getByStudent(studentId: number | string): Promise<WeightRecordDto[]> {
      return requestClient.get<WeightRecordDto[]>(`/health-archive/weight/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<WeightRecordDto> {
      return requestClient.get<WeightRecordDto>(`/health-archive/weight/latest/${studentId}`);
    },
    async create(dto: Omit<WeightRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<WeightRecordDto> {
      return requestClient.post<WeightRecordDto>('/health-archive/weight', dto);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/weight/${id}`);
    },
  },

  // ---- 骨骼 ----
  bone: {
    async getByStudent(studentId: number | string): Promise<BoneRecordDto[]> {
      return requestClient.get<BoneRecordDto[]>(`/health-archive/bone/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<BoneRecordDto> {
      return requestClient.get<BoneRecordDto>(`/health-archive/bone/latest/${studentId}`);
    },
    async create(dto: Omit<BoneRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<BoneRecordDto> {
      return requestClient.post<BoneRecordDto>('/health-archive/bone', dto);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/bone/${id}`);
    },
  },
};
