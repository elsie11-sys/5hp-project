/**
 * 视力档案 - 学生健康档案 API
 *
 * 5 个健康维度：视力 / 口腔 / 心理 / 体重 / 骨骼
 * 包含：分页查询、统计、状态变更、按学生查历史、新增、编辑、删除
 *
 * 后端真实路由前缀：/api/health-archive
 */
import { requestClient } from '#/api/request';

// =========================================================
// 通用类型
// =========================================================
export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface CollectQuery {
  page?: number;
  pageSize?: number;
  status?: 'all' | 'pending' | 'approved' | 'abnormal' | 'spot';
  keyword?: string;
  grade?: string;
  className?: string;
}

export interface CollectStats {
  total: number;
  pending: number;
  approved: number;
  abnormal: number;
  spotCheck: number;
  deviceIngested: number;
  excelImported: number;
}

export interface StatusChangeRequest {
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewRemark?: string;
}

// =========================================================
// 5 维度 DTO
// =========================================================
export interface VisionRecordDto {
  id: number;
  studentId: number;
  studentNo: string;
  studentName: string;
  grade: string;
  className: string;
  checkDate: string;
  leftEye: string;
  rightEye: string;
  visionLevel: string;
  recorderId: number;
  recorderName: string;
  recordTime: string;
  source: 'manual' | 'excel' | 'device';
  deviceSn?: string;
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewTime?: string;
  reviewRemark?: string;
  remark?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface OralRecordDto {
  id: number;
  studentId: number;
  studentNo: string;
  studentName: string;
  grade: string;
  className: string;
  checkDate: string;
  decayedBabyTeeth: number;
  decayedPermanentTeeth: number;
  toothStage: string;
  jawDevelopment: string;
  recorderId: number;
  recorderName: string;
  recordTime: string;
  source: 'manual' | 'excel' | 'device';
  deviceSn?: string;
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewTime?: string;
  reviewRemark?: string;
  remark?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface MentalRecordDto {
  id: number;
  studentId: number;
  studentNo: string;
  studentName: string;
  grade: string;
  className: string;
  checkDate: string;
  anxietyScore: number;
  depressionScore: number;
  learningAnxiety: string;
  interpersonalSensitivity: string;
  recorderId: number;
  recorderName: string;
  recordTime: string;
  source: 'manual' | 'excel' | 'device';
  deviceSn?: string;
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewTime?: string;
  reviewRemark?: string;
  remark?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface WeightRecordDto {
  id: number;
  studentId: number;
  studentNo: string;
  studentName: string;
  grade: string;
  className: string;
  checkDate: string;
  height: string;
  weight: string;
  bmi: string;
  bmiLevel: string;
  waistCircumference: string;
  hipCircumference: string;
  whr: string;
  recorderId: number;
  recorderName: string;
  recordTime: string;
  source: 'manual' | 'excel' | 'device';
  deviceSn?: string;
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewTime?: string;
  reviewRemark?: string;
  remark?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface BoneRecordDto {
  id: number;
  studentId: number;
  studentNo: string;
  studentName: string;
  grade: string;
  className: string;
  checkDate: string;
  boneDensity: string;
  boneLevel: string;
  boneAge: string;
  vitaminD: string;
  calciumLevel: string;
  recorderId: number;
  recorderName: string;
  recordTime: string;
  source: 'manual' | 'excel' | 'device';
  deviceSn?: string;
  status: 'pending' | 'approved' | 'abnormal' | 'spot';
  reviewerId?: number;
  reviewerName?: string;
  reviewTime?: string;
  reviewRemark?: string;
  remark?: string;
  createdAt?: string;
  updatedAt?: string;
}

// =========================================================
// 健康档案 API
// =========================================================

export const healthArchiveApi = {
  /** 5 维度汇总统计（顶部卡片） */
  async getOverallStats(): Promise<CollectStats> {
    return requestClient.get<CollectStats>('/health-archive/stats/overall');
  },

  // ---- 视力 ----
  vision: {
    async getPage(q: CollectQuery): Promise<PagedResult<VisionRecordDto>> {
      return requestClient.get<PagedResult<VisionRecordDto>>('/health-archive/vision/page', { params: q });
    },
    async getStats(): Promise<CollectStats> {
      return requestClient.get<CollectStats>('/health-archive/vision/stats');
    },
    async getByStudent(studentId: number | string): Promise<VisionRecordDto[]> {
      return requestClient.get<VisionRecordDto[]>(`/health-archive/vision/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<VisionRecordDto> {
      return requestClient.get<VisionRecordDto>(`/health-archive/vision/latest/${studentId}`);
    },
    async create(dto: Omit<VisionRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<VisionRecordDto> {
      return requestClient.post<VisionRecordDto>('/health-archive/vision', dto);
    },
    async update(id: number | string, dto: Partial<VisionRecordDto>): Promise<VisionRecordDto> {
      return requestClient.put<VisionRecordDto>(`/health-archive/vision/${id}`, dto);
    },
    async changeStatus(id: number | string, req: StatusChangeRequest): Promise<{ message: string }> {
      return requestClient.post(`/health-archive/vision/${id}/status`, req);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/vision/${id}`);
    },
  },

  // ---- 口腔 ----
  oral: {
    async getPage(q: CollectQuery): Promise<PagedResult<OralRecordDto>> {
      return requestClient.get<PagedResult<OralRecordDto>>('/health-archive/oral/page', { params: q });
    },
    async getStats(): Promise<CollectStats> {
      return requestClient.get<CollectStats>('/health-archive/oral/stats');
    },
    async getByStudent(studentId: number | string): Promise<OralRecordDto[]> {
      return requestClient.get<OralRecordDto[]>(`/health-archive/oral/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<OralRecordDto> {
      return requestClient.get<OralRecordDto>(`/health-archive/oral/latest/${studentId}`);
    },
    async create(dto: Omit<OralRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<OralRecordDto> {
      return requestClient.post<OralRecordDto>('/health-archive/oral', dto);
    },
    async update(id: number | string, dto: Partial<OralRecordDto>): Promise<OralRecordDto> {
      return requestClient.put<OralRecordDto>(`/health-archive/oral/${id}`, dto);
    },
    async changeStatus(id: number | string, req: StatusChangeRequest): Promise<{ message: string }> {
      return requestClient.post(`/health-archive/oral/${id}/status`, req);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/oral/${id}`);
    },
  },

  // ---- 心理 ----
  mental: {
    async getPage(q: CollectQuery): Promise<PagedResult<MentalRecordDto>> {
      return requestClient.get<PagedResult<MentalRecordDto>>('/health-archive/mental/page', { params: q });
    },
    async getStats(): Promise<CollectStats> {
      return requestClient.get<CollectStats>('/health-archive/mental/stats');
    },
    async getByStudent(studentId: number | string): Promise<MentalRecordDto[]> {
      return requestClient.get<MentalRecordDto[]>(`/health-archive/mental/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<MentalRecordDto> {
      return requestClient.get<MentalRecordDto>(`/health-archive/mental/latest/${studentId}`);
    },
    async create(dto: Omit<MentalRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<MentalRecordDto> {
      return requestClient.post<MentalRecordDto>('/health-archive/mental', dto);
    },
    async update(id: number | string, dto: Partial<MentalRecordDto>): Promise<MentalRecordDto> {
      return requestClient.put<MentalRecordDto>(`/health-archive/mental/${id}`, dto);
    },
    async changeStatus(id: number | string, req: StatusChangeRequest): Promise<{ message: string }> {
      return requestClient.post(`/health-archive/mental/${id}/status`, req);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/mental/${id}`);
    },
  },

  // ---- 体重 ----
  weight: {
    async getPage(q: CollectQuery): Promise<PagedResult<WeightRecordDto>> {
      return requestClient.get<PagedResult<WeightRecordDto>>('/health-archive/weight/page', { params: q });
    },
    async getStats(): Promise<CollectStats> {
      return requestClient.get<CollectStats>('/health-archive/weight/stats');
    },
    async getByStudent(studentId: number | string): Promise<WeightRecordDto[]> {
      return requestClient.get<WeightRecordDto[]>(`/health-archive/weight/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<WeightRecordDto> {
      return requestClient.get<WeightRecordDto>(`/health-archive/weight/latest/${studentId}`);
    },
    async create(dto: Omit<WeightRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<WeightRecordDto> {
      return requestClient.post<WeightRecordDto>('/health-archive/weight', dto);
    },
    async update(id: number | string, dto: Partial<WeightRecordDto>): Promise<WeightRecordDto> {
      return requestClient.put<WeightRecordDto>(`/health-archive/weight/${id}`, dto);
    },
    async changeStatus(id: number | string, req: StatusChangeRequest): Promise<{ message: string }> {
      return requestClient.post(`/health-archive/weight/${id}/status`, req);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/weight/${id}`);
    },
  },

  // ---- 骨骼 ----
  bone: {
    async getPage(q: CollectQuery): Promise<PagedResult<BoneRecordDto>> {
      return requestClient.get<PagedResult<BoneRecordDto>>('/health-archive/bone/page', { params: q });
    },
    async getStats(): Promise<CollectStats> {
      return requestClient.get<CollectStats>('/health-archive/bone/stats');
    },
    async getByStudent(studentId: number | string): Promise<BoneRecordDto[]> {
      return requestClient.get<BoneRecordDto[]>(`/health-archive/bone/by-student/${studentId}`);
    },
    async getLatest(studentId: number | string): Promise<BoneRecordDto> {
      return requestClient.get<BoneRecordDto>(`/health-archive/bone/latest/${studentId}`);
    },
    async create(dto: Omit<BoneRecordDto, 'id' | 'createdAt' | 'updatedAt'>): Promise<BoneRecordDto> {
      return requestClient.post<BoneRecordDto>('/health-archive/bone', dto);
    },
    async update(id: number | string, dto: Partial<BoneRecordDto>): Promise<BoneRecordDto> {
      return requestClient.put<BoneRecordDto>(`/health-archive/bone/${id}`, dto);
    },
    async changeStatus(id: number | string, req: StatusChangeRequest): Promise<{ message: string }> {
      return requestClient.post(`/health-archive/bone/${id}/status`, req);
    },
    async remove(id: number | string): Promise<void> {
      await requestClient.delete(`/health-archive/bone/${id}`);
    },
  },
};
