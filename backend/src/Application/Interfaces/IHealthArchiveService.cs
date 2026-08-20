using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 学生健康档案服务接口（数据采集与审核）
/// 包含 5 个健康维度的：分页查询、统计、状态变更、批量导入、设备入库
/// </summary>
public interface IHealthArchiveService
{
    // ---------- 视力 ----------
    Task<List<VisionRecordDto>> GetVisionHistoryAsync(long studentId);
    Task<VisionRecordDto?> GetLatestVisionAsync(long studentId);
    Task<PagedResult<VisionRecordDto>> GetVisionPagedAsync(CollectQueryDto query);
    Task<CollectStatsDto> GetVisionStatsAsync();
    Task<VisionRecordDto> CreateVisionRecordAsync(VisionRecordDto dto);
    Task<VisionRecordDto?> UpdateVisionRecordAsync(long id, VisionRecordDto dto);
    Task<bool> ChangeVisionStatusAsync(long id, StatusChangeRequest req);
    Task<bool> DeleteVisionRecordAsync(long id);
    Task<DeviceIngestResponse> DeviceIngestVisionAsync(DeviceIngestRequest<VisionRecordDto> req);

    // ---------- 口腔 ----------
    Task<List<OralRecordDto>> GetOralHistoryAsync(long studentId);
    Task<OralRecordDto?> GetLatestOralAsync(long studentId);
    Task<PagedResult<OralRecordDto>> GetOralPagedAsync(CollectQueryDto query);
    Task<CollectStatsDto> GetOralStatsAsync();
    Task<OralRecordDto> CreateOralRecordAsync(OralRecordDto dto);
    Task<OralRecordDto?> UpdateOralRecordAsync(long id, OralRecordDto dto);
    Task<bool> ChangeOralStatusAsync(long id, StatusChangeRequest req);
    Task<bool> DeleteOralRecordAsync(long id);
    Task<DeviceIngestResponse> DeviceIngestOralAsync(DeviceIngestRequest<OralRecordDto> req);

    // ---------- 心理 ----------
    Task<List<MentalRecordDto>> GetMentalHistoryAsync(long studentId);
    Task<MentalRecordDto?> GetLatestMentalAsync(long studentId);
    Task<PagedResult<MentalRecordDto>> GetMentalPagedAsync(CollectQueryDto query);
    Task<CollectStatsDto> GetMentalStatsAsync();
    Task<MentalRecordDto> CreateMentalRecordAsync(MentalRecordDto dto);
    Task<MentalRecordDto?> UpdateMentalRecordAsync(long id, MentalRecordDto dto);
    Task<bool> ChangeMentalStatusAsync(long id, StatusChangeRequest req);
    Task<bool> DeleteMentalRecordAsync(long id);
    Task<DeviceIngestResponse> DeviceIngestMentalAsync(DeviceIngestRequest<MentalRecordDto> req);

    // ---------- 体重 ----------
    Task<List<WeightRecordDto>> GetWeightHistoryAsync(long studentId);
    Task<WeightRecordDto?> GetLatestWeightAsync(long studentId);
    Task<PagedResult<WeightRecordDto>> GetWeightPagedAsync(CollectQueryDto query);
    Task<CollectStatsDto> GetWeightStatsAsync();
    Task<WeightRecordDto> CreateWeightRecordAsync(WeightRecordDto dto);
    Task<WeightRecordDto?> UpdateWeightRecordAsync(long id, WeightRecordDto dto);
    Task<bool> ChangeWeightStatusAsync(long id, StatusChangeRequest req);
    Task<bool> DeleteWeightRecordAsync(long id);
    Task<DeviceIngestResponse> DeviceIngestWeightAsync(DeviceIngestRequest<WeightRecordDto> req);

    // ---------- 骨骼 ----------
    Task<List<BoneRecordDto>> GetBoneHistoryAsync(long studentId);
    Task<BoneRecordDto?> GetLatestBoneAsync(long studentId);
    Task<PagedResult<BoneRecordDto>> GetBonePagedAsync(CollectQueryDto query);
    Task<CollectStatsDto> GetBoneStatsAsync();
    Task<BoneRecordDto> CreateBoneRecordAsync(BoneRecordDto dto);
    Task<BoneRecordDto?> UpdateBoneRecordAsync(long id, BoneRecordDto dto);
    Task<bool> ChangeBoneStatusAsync(long id, StatusChangeRequest req);
    Task<bool> DeleteBoneRecordAsync(long id);
    Task<DeviceIngestResponse> DeviceIngestBoneAsync(DeviceIngestRequest<BoneRecordDto> req);

    // ---------- 通用聚合 ----------
    /// <summary>采集与审核页头部统计（5 维度汇总）</summary>
    Task<CollectStatsDto> GetOverallStatsAsync();
}
