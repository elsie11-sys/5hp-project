using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 学生健康档案服务接口
/// 包含 5 个健康维度的历史记录查询、创建、删除
/// </summary>
public interface IHealthArchiveService
{
    // ---------- 视力 ----------
    Task<List<VisionRecordDto>> GetVisionHistoryAsync(long studentId);
    Task<VisionRecordDto?> GetLatestVisionAsync(long studentId);
    Task<VisionRecordDto> CreateVisionRecordAsync(VisionRecordDto dto);
    Task<bool> DeleteVisionRecordAsync(long id);

    // ---------- 口腔 ----------
    Task<List<OralRecordDto>> GetOralHistoryAsync(long studentId);
    Task<OralRecordDto?> GetLatestOralAsync(long studentId);
    Task<OralRecordDto> CreateOralRecordAsync(OralRecordDto dto);
    Task<bool> DeleteOralRecordAsync(long id);

    // ---------- 心理 ----------
    Task<List<MentalRecordDto>> GetMentalHistoryAsync(long studentId);
    Task<MentalRecordDto?> GetLatestMentalAsync(long studentId);
    Task<MentalRecordDto> CreateMentalRecordAsync(MentalRecordDto dto);
    Task<bool> DeleteMentalRecordAsync(long id);

    // ---------- 体重 ----------
    Task<List<WeightRecordDto>> GetWeightHistoryAsync(long studentId);
    Task<WeightRecordDto?> GetLatestWeightAsync(long studentId);
    Task<WeightRecordDto> CreateWeightRecordAsync(WeightRecordDto dto);
    Task<bool> DeleteWeightRecordAsync(long id);

    // ---------- 骨骼 ----------
    Task<List<BoneRecordDto>> GetBoneHistoryAsync(long studentId);
    Task<BoneRecordDto?> GetLatestBoneAsync(long studentId);
    Task<BoneRecordDto> CreateBoneRecordAsync(BoneRecordDto dto);
    Task<bool> DeleteBoneRecordAsync(long id);
}
