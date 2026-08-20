using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 学生健康档案服务实现（数据采集与审核）
/// </summary>
public class HealthArchiveService : IHealthArchiveService
{
    private readonly IApplicationDbContext _context;

    public HealthArchiveService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // 视力
    // ============================================================
    public async Task<List<VisionRecordDto>> GetVisionHistoryAsync(long studentId)
    {
        return await _context.VisionRecords
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate)
            .ThenByDescending(r => r.Id)
            .Select(r => VisionToDto(r))
            .ToListAsync();
    }

    public async Task<VisionRecordDto?> GetLatestVisionAsync(long studentId)
    {
        return (await GetVisionHistoryAsync(studentId)).FirstOrDefault();
    }

    public async Task<PagedResult<VisionRecordDto>> GetVisionPagedAsync(CollectQueryDto q)
    {
        var query = _context.VisionRecords.AsNoTracking();
        query = ApplyCommonFilters(query, q.Keyword, q.Status, q.Grade, q.ClassName);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.RecordTime)
            .ThenByDescending(r => r.Id)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(r => VisionToDto(r))
            .ToListAsync();
        return new PagedResult<VisionRecordDto> { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize };
    }

    public async Task<CollectStatsDto> GetVisionStatsAsync() => await CalcStatsAsync(_context.VisionRecords.AsNoTracking());

    public async Task<VisionRecordDto> CreateVisionRecordAsync(VisionRecordDto dto)
    {
        var entity = VisionToEntity(dto);
        var now = DateTime.Now;
        entity.CreatedAt = now;
        entity.UpdatedAt = now;
        if (entity.RecordTime == default) entity.RecordTime = now;
        if (string.IsNullOrEmpty(entity.Status)) entity.Status = "pending";
        if (string.IsNullOrEmpty(entity.Source)) entity.Source = "manual";
        if (entity.StudentId <= 0)
            entity.StudentId = await ResolveStudentIdAsync(entity.StudentNo, entity.StudentId);
        _context.VisionRecords.Add(entity);
        await _context.SaveChangesAsync();
        return VisionToDto(entity);
    }

    public async Task<VisionRecordDto?> UpdateVisionRecordAsync(long id, VisionRecordDto dto)
    {
        var e = await _context.VisionRecords.FindAsync(id);
        if (e == null) return null;
        e.StudentId = dto.StudentId;
        e.StudentNo = dto.StudentNo;
        e.StudentName = dto.StudentName;
        e.Grade = dto.Grade;
        e.ClassName = dto.ClassName;
        e.CheckDate = ParseDate(dto.CheckDate) ?? e.CheckDate;
        e.LeftEye = dto.LeftEye ?? string.Empty;
        e.RightEye = dto.RightEye ?? string.Empty;
        e.VisionLevel = dto.VisionLevel ?? string.Empty;
        e.Remark = dto.Remark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return VisionToDto(e);
    }

    public async Task<bool> ChangeVisionStatusAsync(long id, StatusChangeRequest req)
    {
        var e = await _context.VisionRecords.FindAsync(id);
        if (e == null) return false;
        e.Status = req.Status;
        e.ReviewerId = req.ReviewerId;
        e.ReviewerName = req.ReviewerName;
        e.ReviewTime = DateTime.Now;
        e.ReviewRemark = req.ReviewRemark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteVisionRecordAsync(long id)
    {
        var e = await _context.VisionRecords.FindAsync(id);
        if (e == null) return false;
        _context.VisionRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeviceIngestResponse> DeviceIngestVisionAsync(DeviceIngestRequest<VisionRecordDto> req)
    {
        var resp = new DeviceIngestResponse { Total = req.Items?.Count ?? 0 };
        if (req.Items == null || req.Items.Count == 0) return resp;
        var now = DateTime.Now;
        var collectedAt = req.CollectedAt ?? now;
        foreach (var dto in req.Items)
        {
            try
            {
                dto.DeviceSn ??= req.DeviceSn;
                dto.Source = "device";
                if (dto.RecordTime == default) dto.RecordTime = collectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(dto.Status)) dto.Status = "pending";
                var entity = VisionToEntity(dto);
                entity.CreatedAt = now;
                entity.UpdatedAt = now;
                if (entity.RecordTime == default) entity.RecordTime = collectedAt;
                if (entity.RecorderId == 0) entity.RecorderId = req.RecorderId ?? 0;
                if (string.IsNullOrEmpty(entity.RecorderName)) entity.RecorderName = req.RecorderName ?? "设备采集";
                if (string.IsNullOrEmpty(entity.DeviceSn)) entity.DeviceSn = req.DeviceSn;
                _context.VisionRecords.Add(entity);
                resp.Success++;
            }
            catch (Exception ex)
            {
                resp.Failed++;
                resp.Errors.Add($"Vision item (student={dto.StudentNo}): {ex.Message}");
            }
        }
        await _context.SaveChangesAsync();
        return resp;
    }

    // ============================================================
    // 口腔
    // ============================================================
    public async Task<List<OralRecordDto>> GetOralHistoryAsync(long studentId)
        => await _context.OralRecords.AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate).ThenByDescending(r => r.Id)
            .Select(r => OralToDto(r)).ToListAsync();

    public async Task<OralRecordDto?> GetLatestOralAsync(long studentId) => (await GetOralHistoryAsync(studentId)).FirstOrDefault();

    public async Task<PagedResult<OralRecordDto>> GetOralPagedAsync(CollectQueryDto q)
    {
        var query = _context.OralRecords.AsNoTracking();
        query = ApplyCommonFilters(query, q.Keyword, q.Status, q.Grade, q.ClassName);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.RecordTime).ThenByDescending(r => r.Id)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).Select(r => OralToDto(r)).ToListAsync();
        return new PagedResult<OralRecordDto> { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize };
    }

    public async Task<CollectStatsDto> GetOralStatsAsync() => await CalcStatsAsync(_context.OralRecords.AsNoTracking());

    public async Task<OralRecordDto> CreateOralRecordAsync(OralRecordDto dto)
    {
        var entity = OralToEntity(dto);
        var now = DateTime.Now;
        entity.CreatedAt = now; entity.UpdatedAt = now;
        if (entity.RecordTime == default) entity.RecordTime = now;
        if (string.IsNullOrEmpty(entity.Status)) entity.Status = "pending";
        if (string.IsNullOrEmpty(entity.Source)) entity.Source = "manual";
        if (entity.StudentId <= 0)
            entity.StudentId = await ResolveStudentIdAsync(entity.StudentNo, entity.StudentId);
        _context.OralRecords.Add(entity);
        await _context.SaveChangesAsync();
        return OralToDto(entity);
    }

    public async Task<OralRecordDto?> UpdateOralRecordAsync(long id, OralRecordDto dto)
    {
        var e = await _context.OralRecords.FindAsync(id);
        if (e == null) return null;
        e.StudentId = dto.StudentId;
        e.StudentNo = dto.StudentNo; e.StudentName = dto.StudentName;
        e.Grade = dto.Grade; e.ClassName = dto.ClassName;
        e.CheckDate = ParseDate(dto.CheckDate) ?? e.CheckDate;
        e.DecayedBabyTeeth = dto.DecayedBabyTeeth;
        e.DecayedPermanentTeeth = dto.DecayedPermanentTeeth;
        e.ToothStage = dto.ToothStage ?? string.Empty;
        e.JawDevelopment = dto.JawDevelopment ?? string.Empty;
        e.Remark = dto.Remark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return OralToDto(e);
    }

    public async Task<bool> ChangeOralStatusAsync(long id, StatusChangeRequest req)
    {
        var e = await _context.OralRecords.FindAsync(id);
        if (e == null) return false;
        e.Status = req.Status;
        e.ReviewerId = req.ReviewerId; e.ReviewerName = req.ReviewerName;
        e.ReviewTime = DateTime.Now; e.ReviewRemark = req.ReviewRemark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(); return true;
    }

    public async Task<bool> DeleteOralRecordAsync(long id)
    {
        var e = await _context.OralRecords.FindAsync(id);
        if (e == null) return false;
        _context.OralRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeviceIngestResponse> DeviceIngestOralAsync(DeviceIngestRequest<OralRecordDto> req)
    {
        var resp = new DeviceIngestResponse { Total = req.Items?.Count ?? 0 };
        if (req.Items == null || req.Items.Count == 0) return resp;
        var now = DateTime.Now;
        var collectedAt = req.CollectedAt ?? now;
        foreach (var dto in req.Items)
        {
            try
            {
                dto.DeviceSn ??= req.DeviceSn; dto.Source = "device";
                if (dto.RecordTime == default) dto.RecordTime = collectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(dto.Status)) dto.Status = "pending";
                var entity = OralToEntity(dto);
                entity.CreatedAt = now; entity.UpdatedAt = now;
                if (entity.RecordTime == default) entity.RecordTime = collectedAt;
                if (entity.RecorderId == 0) entity.RecorderId = req.RecorderId ?? 0;
                if (string.IsNullOrEmpty(entity.RecorderName)) entity.RecorderName = req.RecorderName ?? "设备采集";
                if (string.IsNullOrEmpty(entity.DeviceSn)) entity.DeviceSn = req.DeviceSn;
                _context.OralRecords.Add(entity);
                resp.Success++;
            }
            catch (Exception ex) { resp.Failed++; resp.Errors.Add($"Oral item: {ex.Message}"); }
        }
        await _context.SaveChangesAsync();
        return resp;
    }

    // ============================================================
    // 心理
    // ============================================================
    public async Task<List<MentalRecordDto>> GetMentalHistoryAsync(long studentId)
        => await _context.MentalRecords.AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate).ThenByDescending(r => r.Id)
            .Select(r => MentalToDto(r)).ToListAsync();

    public async Task<MentalRecordDto?> GetLatestMentalAsync(long studentId) => (await GetMentalHistoryAsync(studentId)).FirstOrDefault();

    public async Task<PagedResult<MentalRecordDto>> GetMentalPagedAsync(CollectQueryDto q)
    {
        var query = _context.MentalRecords.AsNoTracking();
        query = ApplyCommonFilters(query, q.Keyword, q.Status, q.Grade, q.ClassName);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.RecordTime).ThenByDescending(r => r.Id)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).Select(r => MentalToDto(r)).ToListAsync();
        return new PagedResult<MentalRecordDto> { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize };
    }

    public async Task<CollectStatsDto> GetMentalStatsAsync() => await CalcStatsAsync(_context.MentalRecords.AsNoTracking());

    public async Task<MentalRecordDto> CreateMentalRecordAsync(MentalRecordDto dto)
    {
        var entity = MentalToEntity(dto);
        var now = DateTime.Now;
        entity.CreatedAt = now; entity.UpdatedAt = now;
        if (entity.RecordTime == default) entity.RecordTime = now;
        if (string.IsNullOrEmpty(entity.Status)) entity.Status = "pending";
        if (string.IsNullOrEmpty(entity.Source)) entity.Source = "manual";
        if (entity.StudentId <= 0)
            entity.StudentId = await ResolveStudentIdAsync(entity.StudentNo, entity.StudentId);
        _context.MentalRecords.Add(entity);
        await _context.SaveChangesAsync();
        return MentalToDto(entity);
    }

    public async Task<MentalRecordDto?> UpdateMentalRecordAsync(long id, MentalRecordDto dto)
    {
        var e = await _context.MentalRecords.FindAsync(id);
        if (e == null) return null;
        e.StudentId = dto.StudentId;
        e.StudentNo = dto.StudentNo; e.StudentName = dto.StudentName;
        e.Grade = dto.Grade; e.ClassName = dto.ClassName;
        e.CheckDate = ParseDate(dto.CheckDate) ?? e.CheckDate;
        e.AnxietyScore = dto.AnxietyScore;
        e.DepressionScore = dto.DepressionScore;
        e.LearningAnxiety = dto.LearningAnxiety ?? string.Empty;
        e.InterpersonalSensitivity = dto.InterpersonalSensitivity ?? string.Empty;
        e.Remark = dto.Remark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return MentalToDto(e);
    }

    public async Task<bool> ChangeMentalStatusAsync(long id, StatusChangeRequest req)
    {
        var e = await _context.MentalRecords.FindAsync(id);
        if (e == null) return false;
        e.Status = req.Status;
        e.ReviewerId = req.ReviewerId; e.ReviewerName = req.ReviewerName;
        e.ReviewTime = DateTime.Now; e.ReviewRemark = req.ReviewRemark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(); return true;
    }

    public async Task<bool> DeleteMentalRecordAsync(long id)
    {
        var e = await _context.MentalRecords.FindAsync(id);
        if (e == null) return false;
        _context.MentalRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeviceIngestResponse> DeviceIngestMentalAsync(DeviceIngestRequest<MentalRecordDto> req)
    {
        var resp = new DeviceIngestResponse { Total = req.Items?.Count ?? 0 };
        if (req.Items == null || req.Items.Count == 0) return resp;
        var now = DateTime.Now;
        var collectedAt = req.CollectedAt ?? now;
        foreach (var dto in req.Items)
        {
            try
            {
                dto.DeviceSn ??= req.DeviceSn; dto.Source = "device";
                if (dto.RecordTime == default) dto.RecordTime = collectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(dto.Status)) dto.Status = "pending";
                var entity = MentalToEntity(dto);
                entity.CreatedAt = now; entity.UpdatedAt = now;
                if (entity.RecordTime == default) entity.RecordTime = collectedAt;
                if (entity.RecorderId == 0) entity.RecorderId = req.RecorderId ?? 0;
                if (string.IsNullOrEmpty(entity.RecorderName)) entity.RecorderName = req.RecorderName ?? "设备采集";
                if (string.IsNullOrEmpty(entity.DeviceSn)) entity.DeviceSn = req.DeviceSn;
                _context.MentalRecords.Add(entity);
                resp.Success++;
            }
            catch (Exception ex) { resp.Failed++; resp.Errors.Add($"Mental item: {ex.Message}"); }
        }
        await _context.SaveChangesAsync();
        return resp;
    }

    // ============================================================
    // 体重
    // ============================================================
    public async Task<List<WeightRecordDto>> GetWeightHistoryAsync(long studentId)
        => await _context.WeightRecords.AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate).ThenByDescending(r => r.Id)
            .Select(r => WeightToDto(r)).ToListAsync();

    public async Task<WeightRecordDto?> GetLatestWeightAsync(long studentId) => (await GetWeightHistoryAsync(studentId)).FirstOrDefault();

    public async Task<PagedResult<WeightRecordDto>> GetWeightPagedAsync(CollectQueryDto q)
    {
        var query = _context.WeightRecords.AsNoTracking();
        query = ApplyCommonFilters(query, q.Keyword, q.Status, q.Grade, q.ClassName);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.RecordTime).ThenByDescending(r => r.Id)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).Select(r => WeightToDto(r)).ToListAsync();
        return new PagedResult<WeightRecordDto> { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize };
    }

    public async Task<CollectStatsDto> GetWeightStatsAsync() => await CalcStatsAsync(_context.WeightRecords.AsNoTracking());

    public async Task<WeightRecordDto> CreateWeightRecordAsync(WeightRecordDto dto)
    {
        var entity = WeightToEntity(dto);
        var now = DateTime.Now;
        entity.CreatedAt = now; entity.UpdatedAt = now;
        if (entity.RecordTime == default) entity.RecordTime = now;
        if (string.IsNullOrEmpty(entity.Status)) entity.Status = "pending";
        if (string.IsNullOrEmpty(entity.Source)) entity.Source = "manual";
        if (entity.StudentId <= 0)
            entity.StudentId = await ResolveStudentIdAsync(entity.StudentNo, entity.StudentId);
        _context.WeightRecords.Add(entity);
        await _context.SaveChangesAsync();
        return WeightToDto(entity);
    }

    public async Task<WeightRecordDto?> UpdateWeightRecordAsync(long id, WeightRecordDto dto)
    {
        var e = await _context.WeightRecords.FindAsync(id);
        if (e == null) return null;
        e.StudentId = dto.StudentId;
        e.StudentNo = dto.StudentNo; e.StudentName = dto.StudentName;
        e.Grade = dto.Grade; e.ClassName = dto.ClassName;
        e.CheckDate = ParseDate(dto.CheckDate) ?? e.CheckDate;
        e.Height = dto.Height ?? string.Empty;
        e.Weight = dto.Weight ?? string.Empty;
        e.Bmi = dto.Bmi ?? string.Empty;
        e.BmiLevel = dto.BmiLevel ?? string.Empty;
        e.WaistCircumference = dto.WaistCircumference ?? string.Empty;
        e.HipCircumference = dto.HipCircumference ?? string.Empty;
        e.Whr = dto.Whr ?? string.Empty;
        e.Remark = dto.Remark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return WeightToDto(e);
    }

    public async Task<bool> ChangeWeightStatusAsync(long id, StatusChangeRequest req)
    {
        var e = await _context.WeightRecords.FindAsync(id);
        if (e == null) return false;
        e.Status = req.Status;
        e.ReviewerId = req.ReviewerId; e.ReviewerName = req.ReviewerName;
        e.ReviewTime = DateTime.Now; e.ReviewRemark = req.ReviewRemark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(); return true;
    }

    public async Task<bool> DeleteWeightRecordAsync(long id)
    {
        var e = await _context.WeightRecords.FindAsync(id);
        if (e == null) return false;
        _context.WeightRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeviceIngestResponse> DeviceIngestWeightAsync(DeviceIngestRequest<WeightRecordDto> req)
    {
        var resp = new DeviceIngestResponse { Total = req.Items?.Count ?? 0 };
        if (req.Items == null || req.Items.Count == 0) return resp;
        var now = DateTime.Now;
        var collectedAt = req.CollectedAt ?? now;
        foreach (var dto in req.Items)
        {
            try
            {
                dto.DeviceSn ??= req.DeviceSn; dto.Source = "device";
                if (dto.RecordTime == default) dto.RecordTime = collectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(dto.Status)) dto.Status = "pending";
                var entity = WeightToEntity(dto);
                entity.CreatedAt = now; entity.UpdatedAt = now;
                if (entity.RecordTime == default) entity.RecordTime = collectedAt;
                if (entity.RecorderId == 0) entity.RecorderId = req.RecorderId ?? 0;
                if (string.IsNullOrEmpty(entity.RecorderName)) entity.RecorderName = req.RecorderName ?? "设备采集";
                if (string.IsNullOrEmpty(entity.DeviceSn)) entity.DeviceSn = req.DeviceSn;
                _context.WeightRecords.Add(entity);
                resp.Success++;
            }
            catch (Exception ex) { resp.Failed++; resp.Errors.Add($"Weight item: {ex.Message}"); }
        }
        await _context.SaveChangesAsync();
        return resp;
    }

    // ============================================================
    // 骨骼
    // ============================================================
    public async Task<List<BoneRecordDto>> GetBoneHistoryAsync(long studentId)
        => await _context.BoneRecords.AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate).ThenByDescending(r => r.Id)
            .Select(r => BoneToDto(r)).ToListAsync();

    public async Task<BoneRecordDto?> GetLatestBoneAsync(long studentId) => (await GetBoneHistoryAsync(studentId)).FirstOrDefault();

    public async Task<PagedResult<BoneRecordDto>> GetBonePagedAsync(CollectQueryDto q)
    {
        var query = _context.BoneRecords.AsNoTracking();
        query = ApplyCommonFilters(query, q.Keyword, q.Status, q.Grade, q.ClassName);
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(r => r.RecordTime).ThenByDescending(r => r.Id)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).Select(r => BoneToDto(r)).ToListAsync();
        return new PagedResult<BoneRecordDto> { Items = items, Total = total, Page = q.Page, PageSize = q.PageSize };
    }

    public async Task<CollectStatsDto> GetBoneStatsAsync() => await CalcStatsAsync(_context.BoneRecords.AsNoTracking());

    public async Task<BoneRecordDto> CreateBoneRecordAsync(BoneRecordDto dto)
    {
        var entity = BoneToEntity(dto);
        var now = DateTime.Now;
        entity.CreatedAt = now; entity.UpdatedAt = now;
        if (entity.RecordTime == default) entity.RecordTime = now;
        if (string.IsNullOrEmpty(entity.Status)) entity.Status = "pending";
        if (string.IsNullOrEmpty(entity.Source)) entity.Source = "manual";
        if (entity.StudentId <= 0)
            entity.StudentId = await ResolveStudentIdAsync(entity.StudentNo, entity.StudentId);
        _context.BoneRecords.Add(entity);
        await _context.SaveChangesAsync();
        return BoneToDto(entity);
    }

    public async Task<BoneRecordDto?> UpdateBoneRecordAsync(long id, BoneRecordDto dto)
    {
        var e = await _context.BoneRecords.FindAsync(id);
        if (e == null) return null;
        e.StudentId = dto.StudentId;
        e.StudentNo = dto.StudentNo; e.StudentName = dto.StudentName;
        e.Grade = dto.Grade; e.ClassName = dto.ClassName;
        e.CheckDate = ParseDate(dto.CheckDate) ?? e.CheckDate;
        e.BoneDensity = dto.BoneDensity ?? string.Empty;
        e.BoneLevel = dto.BoneLevel ?? string.Empty;
        e.BoneAge = dto.BoneAge ?? string.Empty;
        e.VitaminD = dto.VitaminD ?? string.Empty;
        e.CalciumLevel = dto.CalciumLevel ?? string.Empty;
        e.Remark = dto.Remark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return BoneToDto(e);
    }

    public async Task<bool> ChangeBoneStatusAsync(long id, StatusChangeRequest req)
    {
        var e = await _context.BoneRecords.FindAsync(id);
        if (e == null) return false;
        e.Status = req.Status;
        e.ReviewerId = req.ReviewerId; e.ReviewerName = req.ReviewerName;
        e.ReviewTime = DateTime.Now; e.ReviewRemark = req.ReviewRemark;
        e.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync(); return true;
    }

    public async Task<bool> DeleteBoneRecordAsync(long id)
    {
        var e = await _context.BoneRecords.FindAsync(id);
        if (e == null) return false;
        _context.BoneRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeviceIngestResponse> DeviceIngestBoneAsync(DeviceIngestRequest<BoneRecordDto> req)
    {
        var resp = new DeviceIngestResponse { Total = req.Items?.Count ?? 0 };
        if (req.Items == null || req.Items.Count == 0) return resp;
        var now = DateTime.Now;
        var collectedAt = req.CollectedAt ?? now;
        foreach (var dto in req.Items)
        {
            try
            {
                dto.DeviceSn ??= req.DeviceSn; dto.Source = "device";
                if (dto.RecordTime == default) dto.RecordTime = collectedAt.ToString("yyyy-MM-dd HH:mm:ss");
                if (string.IsNullOrEmpty(dto.Status)) dto.Status = "pending";
                var entity = BoneToEntity(dto);
                entity.CreatedAt = now; entity.UpdatedAt = now;
                if (entity.RecordTime == default) entity.RecordTime = collectedAt;
                if (entity.RecorderId == 0) entity.RecorderId = req.RecorderId ?? 0;
                if (string.IsNullOrEmpty(entity.RecorderName)) entity.RecorderName = req.RecorderName ?? "设备采集";
                if (string.IsNullOrEmpty(entity.DeviceSn)) entity.DeviceSn = req.DeviceSn;
                _context.BoneRecords.Add(entity);
                resp.Success++;
            }
            catch (Exception ex) { resp.Failed++; resp.Errors.Add($"Bone item: {ex.Message}"); }
        }
        await _context.SaveChangesAsync();
        return resp;
    }

    // ============================================================
    // 总体统计
    // ============================================================
    public async Task<CollectStatsDto> GetOverallStatsAsync()
    {
        var stats = new CollectStatsDto();
        // 5 维度逐个统计（不能用 cast，因为 entity 类型不同）
        stats.Total += await _context.VisionRecords.CountAsync();
        stats.Pending += await _context.VisionRecords.CountAsync(r => r.Status == "pending");
        stats.Approved += await _context.VisionRecords.CountAsync(r => r.Status == "approved");
        stats.Abnormal += await _context.VisionRecords.CountAsync(r => r.Status == "abnormal");
        stats.SpotCheck += await _context.VisionRecords.CountAsync(r => r.Status == "spot");
        stats.DeviceIngested += await _context.VisionRecords.CountAsync(r => r.Source == "device");
        stats.ExcelImported += await _context.VisionRecords.CountAsync(r => r.Source == "excel");

        stats.Total += await _context.OralRecords.CountAsync();
        stats.Pending += await _context.OralRecords.CountAsync(r => r.Status == "pending");
        stats.Approved += await _context.OralRecords.CountAsync(r => r.Status == "approved");
        stats.Abnormal += await _context.OralRecords.CountAsync(r => r.Status == "abnormal");
        stats.SpotCheck += await _context.OralRecords.CountAsync(r => r.Status == "spot");
        stats.DeviceIngested += await _context.OralRecords.CountAsync(r => r.Source == "device");
        stats.ExcelImported += await _context.OralRecords.CountAsync(r => r.Source == "excel");

        stats.Total += await _context.MentalRecords.CountAsync();
        stats.Pending += await _context.MentalRecords.CountAsync(r => r.Status == "pending");
        stats.Approved += await _context.MentalRecords.CountAsync(r => r.Status == "approved");
        stats.Abnormal += await _context.MentalRecords.CountAsync(r => r.Status == "abnormal");
        stats.SpotCheck += await _context.MentalRecords.CountAsync(r => r.Status == "spot");
        stats.DeviceIngested += await _context.MentalRecords.CountAsync(r => r.Source == "device");
        stats.ExcelImported += await _context.MentalRecords.CountAsync(r => r.Source == "excel");

        stats.Total += await _context.WeightRecords.CountAsync();
        stats.Pending += await _context.WeightRecords.CountAsync(r => r.Status == "pending");
        stats.Approved += await _context.WeightRecords.CountAsync(r => r.Status == "approved");
        stats.Abnormal += await _context.WeightRecords.CountAsync(r => r.Status == "abnormal");
        stats.SpotCheck += await _context.WeightRecords.CountAsync(r => r.Status == "spot");
        stats.DeviceIngested += await _context.WeightRecords.CountAsync(r => r.Source == "device");
        stats.ExcelImported += await _context.WeightRecords.CountAsync(r => r.Source == "excel");

        stats.Total += await _context.BoneRecords.CountAsync();
        stats.Pending += await _context.BoneRecords.CountAsync(r => r.Status == "pending");
        stats.Approved += await _context.BoneRecords.CountAsync(r => r.Status == "approved");
        stats.Abnormal += await _context.BoneRecords.CountAsync(r => r.Status == "abnormal");
        stats.SpotCheck += await _context.BoneRecords.CountAsync(r => r.Status == "spot");
        stats.DeviceIngested += await _context.BoneRecords.CountAsync(r => r.Source == "device");
        stats.ExcelImported += await _context.BoneRecords.CountAsync(r => r.Source == "excel");

        return stats;
    }

    /// <summary>
    /// 根据 studentNo 自动从 student_archive 表查 ID（前端不必手填 StudentId）
    /// 查不到则保持 dto 传入的 StudentId（设备推流时设备方可能直接给 ID）
    /// </summary>
    private async Task<long> ResolveStudentIdAsync(string? studentNo, long fallback)
    {
        if (string.IsNullOrWhiteSpace(studentNo)) return fallback;
        var stu = await _context.Students.AsNoTracking()
            .Where(s => s.StudentNo == studentNo)
            .Select(s => (long?)s.Id)
            .FirstOrDefaultAsync();
        return stu ?? fallback;
    }

    // ============================================================
    // 通用工具
    // ============================================================
    private static async Task<CollectStatsDto> CalcStatsAsync(IQueryable<VisionRecord> q)
    {
        return await Task.FromResult(new CollectStatsDto
        {
            Total = q.Count(),
            Pending = q.Count(r => r.Status == "pending"),
            Approved = q.Count(r => r.Status == "approved"),
            Abnormal = q.Count(r => r.Status == "abnormal"),
            SpotCheck = q.Count(r => r.Status == "spot"),
            DeviceIngested = q.Count(r => r.Source == "device"),
            ExcelImported = q.Count(r => r.Source == "excel"),
        });
    }

    private static async Task<CollectStatsDto> CalcStatsAsync(IQueryable<OralRecord> q)
    {
        return await Task.FromResult(new CollectStatsDto
        {
            Total = q.Count(),
            Pending = q.Count(r => r.Status == "pending"),
            Approved = q.Count(r => r.Status == "approved"),
            Abnormal = q.Count(r => r.Status == "abnormal"),
            SpotCheck = q.Count(r => r.Status == "spot"),
            DeviceIngested = q.Count(r => r.Source == "device"),
            ExcelImported = q.Count(r => r.Source == "excel"),
        });
    }

    private static async Task<CollectStatsDto> CalcStatsAsync(IQueryable<MentalRecord> q)
    {
        return await Task.FromResult(new CollectStatsDto
        {
            Total = q.Count(),
            Pending = q.Count(r => r.Status == "pending"),
            Approved = q.Count(r => r.Status == "approved"),
            Abnormal = q.Count(r => r.Status == "abnormal"),
            SpotCheck = q.Count(r => r.Status == "spot"),
            DeviceIngested = q.Count(r => r.Source == "device"),
            ExcelImported = q.Count(r => r.Source == "excel"),
        });
    }

    private static async Task<CollectStatsDto> CalcStatsAsync(IQueryable<WeightRecord> q)
    {
        return await Task.FromResult(new CollectStatsDto
        {
            Total = q.Count(),
            Pending = q.Count(r => r.Status == "pending"),
            Approved = q.Count(r => r.Status == "approved"),
            Abnormal = q.Count(r => r.Status == "abnormal"),
            SpotCheck = q.Count(r => r.Status == "spot"),
            DeviceIngested = q.Count(r => r.Source == "device"),
            ExcelImported = q.Count(r => r.Source == "excel"),
        });
    }

    private static async Task<CollectStatsDto> CalcStatsAsync(IQueryable<BoneRecord> q)
    {
        return await Task.FromResult(new CollectStatsDto
        {
            Total = q.Count(),
            Pending = q.Count(r => r.Status == "pending"),
            Approved = q.Count(r => r.Status == "approved"),
            Abnormal = q.Count(r => r.Status == "abnormal"),
            SpotCheck = q.Count(r => r.Status == "spot"),
            DeviceIngested = q.Count(r => r.Source == "device"),
            ExcelImported = q.Count(r => r.Source == "excel"),
        });
    }

    private static IQueryable<T> ApplyCommonFilters<T>(IQueryable<T> q, string? keyword, string? status, string? grade, string? className) where T : class
    {
        if (!string.IsNullOrEmpty(status) && status != "all")
            q = q.Where(x => EF.Property<string>(x, "Status") == status);
        if (!string.IsNullOrEmpty(grade))
            q = q.Where(x => EF.Property<string>(x, "Grade") == grade);
        if (!string.IsNullOrEmpty(className))
            q = q.Where(x => EF.Property<string>(x, "ClassName") == className);
        if (!string.IsNullOrEmpty(keyword))
        {
            var kw = keyword.Trim();
            q = q.Where(x =>
                EF.Property<string>(x, "StudentNo").Contains(kw) ||
                EF.Property<string>(x, "StudentName").Contains(kw));
        }
        return q;
    }

    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, out var d)) return d;
        return null;
    }

    // ============================================================
    // Entity <-> DTO 映射
    // ============================================================
    private static VisionRecordDto VisionToDto(VisionRecord r) => new()
    {
        Id = r.Id, StudentId = r.StudentId,
        StudentNo = r.StudentNo, StudentName = r.StudentName,
        Grade = r.Grade, ClassName = r.ClassName,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        LeftEye = r.LeftEye, RightEye = r.RightEye, VisionLevel = r.VisionLevel,
        RecorderId = r.RecorderId, RecorderName = r.RecorderName,
        RecordTime = r.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
        Source = r.Source, DeviceSn = r.DeviceSn,
        Status = r.Status, ReviewerId = r.ReviewerId, ReviewerName = r.ReviewerName,
        ReviewTime = r.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
        ReviewRemark = r.ReviewRemark, Remark = r.Remark,
        CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
    };

    private static VisionRecord VisionToEntity(VisionRecordDto dto)
    {
        var e = new VisionRecord
        {
            StudentId = dto.StudentId,
            StudentNo = dto.StudentNo ?? string.Empty,
            StudentName = dto.StudentName ?? string.Empty,
            Grade = dto.Grade ?? string.Empty,
            ClassName = dto.ClassName ?? string.Empty,
            CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
            LeftEye = dto.LeftEye ?? string.Empty,
            RightEye = dto.RightEye ?? string.Empty,
            VisionLevel = dto.VisionLevel ?? string.Empty,
            RecorderId = dto.RecorderId,
            RecorderName = dto.RecorderName ?? string.Empty,
            RecordTime = ParseDate(dto.RecordTime) ?? DateTime.Now,
            Source = dto.Source ?? "manual",
            DeviceSn = dto.DeviceSn,
            Status = dto.Status ?? "pending",
            ReviewerId = dto.ReviewerId,
            ReviewerName = dto.ReviewerName,
            ReviewTime = ParseDate(dto.ReviewTime),
            ReviewRemark = dto.ReviewRemark,
            Remark = dto.Remark,
        };
        if (dto.Id > 0) e.Id = dto.Id;
        return e;
    }

    private static OralRecordDto OralToDto(OralRecord r) => new()
    {
        Id = r.Id, StudentId = r.StudentId,
        StudentNo = r.StudentNo, StudentName = r.StudentName,
        Grade = r.Grade, ClassName = r.ClassName,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        DecayedBabyTeeth = r.DecayedBabyTeeth, DecayedPermanentTeeth = r.DecayedPermanentTeeth,
        ToothStage = r.ToothStage, JawDevelopment = r.JawDevelopment,
        RecorderId = r.RecorderId, RecorderName = r.RecorderName,
        RecordTime = r.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
        Source = r.Source, DeviceSn = r.DeviceSn,
        Status = r.Status, ReviewerId = r.ReviewerId, ReviewerName = r.ReviewerName,
        ReviewTime = r.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
        ReviewRemark = r.ReviewRemark, Remark = r.Remark,
        CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
    };

    private static OralRecord OralToEntity(OralRecordDto dto) => new()
    {
        Id = dto.Id > 0 ? dto.Id : 0, StudentId = dto.StudentId,
        StudentNo = dto.StudentNo ?? string.Empty,
        StudentName = dto.StudentName ?? string.Empty,
        Grade = dto.Grade ?? string.Empty,
        ClassName = dto.ClassName ?? string.Empty,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        DecayedBabyTeeth = dto.DecayedBabyTeeth,
        DecayedPermanentTeeth = dto.DecayedPermanentTeeth,
        ToothStage = dto.ToothStage ?? string.Empty,
        JawDevelopment = dto.JawDevelopment ?? string.Empty,
        RecorderId = dto.RecorderId, RecorderName = dto.RecorderName ?? string.Empty,
        RecordTime = ParseDate(dto.RecordTime) ?? DateTime.Now,
        Source = dto.Source ?? "manual", DeviceSn = dto.DeviceSn,
        Status = dto.Status ?? "pending",
        ReviewerId = dto.ReviewerId, ReviewerName = dto.ReviewerName,
        ReviewTime = ParseDate(dto.ReviewTime), ReviewRemark = dto.ReviewRemark, Remark = dto.Remark,
    };

    private static MentalRecordDto MentalToDto(MentalRecord r) => new()
    {
        Id = r.Id, StudentId = r.StudentId,
        StudentNo = r.StudentNo, StudentName = r.StudentName,
        Grade = r.Grade, ClassName = r.ClassName,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        AnxietyScore = r.AnxietyScore, DepressionScore = r.DepressionScore,
        LearningAnxiety = r.LearningAnxiety, InterpersonalSensitivity = r.InterpersonalSensitivity,
        RecorderId = r.RecorderId, RecorderName = r.RecorderName,
        RecordTime = r.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
        Source = r.Source, DeviceSn = r.DeviceSn,
        Status = r.Status, ReviewerId = r.ReviewerId, ReviewerName = r.ReviewerName,
        ReviewTime = r.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
        ReviewRemark = r.ReviewRemark, Remark = r.Remark,
        CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
    };

    private static MentalRecord MentalToEntity(MentalRecordDto dto) => new()
    {
        Id = dto.Id > 0 ? dto.Id : 0, StudentId = dto.StudentId,
        StudentNo = dto.StudentNo ?? string.Empty,
        StudentName = dto.StudentName ?? string.Empty,
        Grade = dto.Grade ?? string.Empty,
        ClassName = dto.ClassName ?? string.Empty,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        AnxietyScore = dto.AnxietyScore, DepressionScore = dto.DepressionScore,
        LearningAnxiety = dto.LearningAnxiety ?? string.Empty,
        InterpersonalSensitivity = dto.InterpersonalSensitivity ?? string.Empty,
        RecorderId = dto.RecorderId, RecorderName = dto.RecorderName ?? string.Empty,
        RecordTime = ParseDate(dto.RecordTime) ?? DateTime.Now,
        Source = dto.Source ?? "manual", DeviceSn = dto.DeviceSn,
        Status = dto.Status ?? "pending",
        ReviewerId = dto.ReviewerId, ReviewerName = dto.ReviewerName,
        ReviewTime = ParseDate(dto.ReviewTime), ReviewRemark = dto.ReviewRemark, Remark = dto.Remark,
    };

    private static WeightRecordDto WeightToDto(WeightRecord r) => new()
    {
        Id = r.Id, StudentId = r.StudentId,
        StudentNo = r.StudentNo, StudentName = r.StudentName,
        Grade = r.Grade, ClassName = r.ClassName,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        Height = r.Height, Weight = r.Weight, Bmi = r.Bmi, BmiLevel = r.BmiLevel,
        WaistCircumference = r.WaistCircumference, HipCircumference = r.HipCircumference, Whr = r.Whr,
        RecorderId = r.RecorderId, RecorderName = r.RecorderName,
        RecordTime = r.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
        Source = r.Source, DeviceSn = r.DeviceSn,
        Status = r.Status, ReviewerId = r.ReviewerId, ReviewerName = r.ReviewerName,
        ReviewTime = r.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
        ReviewRemark = r.ReviewRemark, Remark = r.Remark,
        CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
    };

    private static WeightRecord WeightToEntity(WeightRecordDto dto) => new()
    {
        Id = dto.Id > 0 ? dto.Id : 0, StudentId = dto.StudentId,
        StudentNo = dto.StudentNo ?? string.Empty,
        StudentName = dto.StudentName ?? string.Empty,
        Grade = dto.Grade ?? string.Empty,
        ClassName = dto.ClassName ?? string.Empty,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        Height = dto.Height ?? string.Empty,
        Weight = dto.Weight ?? string.Empty,
        Bmi = dto.Bmi ?? string.Empty,
        BmiLevel = dto.BmiLevel ?? string.Empty,
        WaistCircumference = dto.WaistCircumference ?? string.Empty,
        HipCircumference = dto.HipCircumference ?? string.Empty,
        Whr = dto.Whr ?? string.Empty,
        RecorderId = dto.RecorderId, RecorderName = dto.RecorderName ?? string.Empty,
        RecordTime = ParseDate(dto.RecordTime) ?? DateTime.Now,
        Source = dto.Source ?? "manual", DeviceSn = dto.DeviceSn,
        Status = dto.Status ?? "pending",
        ReviewerId = dto.ReviewerId, ReviewerName = dto.ReviewerName,
        ReviewTime = ParseDate(dto.ReviewTime), ReviewRemark = dto.ReviewRemark, Remark = dto.Remark,
    };

    private static BoneRecordDto BoneToDto(BoneRecord r) => new()
    {
        Id = r.Id, StudentId = r.StudentId,
        StudentNo = r.StudentNo, StudentName = r.StudentName,
        Grade = r.Grade, ClassName = r.ClassName,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        BoneDensity = r.BoneDensity, BoneLevel = r.BoneLevel, BoneAge = r.BoneAge,
        VitaminD = r.VitaminD, CalciumLevel = r.CalciumLevel,
        RecorderId = r.RecorderId, RecorderName = r.RecorderName,
        RecordTime = r.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"),
        Source = r.Source, DeviceSn = r.DeviceSn,
        Status = r.Status, ReviewerId = r.ReviewerId, ReviewerName = r.ReviewerName,
        ReviewTime = r.ReviewTime?.ToString("yyyy-MM-dd HH:mm:ss"),
        ReviewRemark = r.ReviewRemark, Remark = r.Remark,
        CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt,
    };

    private static BoneRecord BoneToEntity(BoneRecordDto dto) => new()
    {
        Id = dto.Id > 0 ? dto.Id : 0, StudentId = dto.StudentId,
        StudentNo = dto.StudentNo ?? string.Empty,
        StudentName = dto.StudentName ?? string.Empty,
        Grade = dto.Grade ?? string.Empty,
        ClassName = dto.ClassName ?? string.Empty,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        BoneDensity = dto.BoneDensity ?? string.Empty,
        BoneLevel = dto.BoneLevel ?? string.Empty,
        BoneAge = dto.BoneAge ?? string.Empty,
        VitaminD = dto.VitaminD ?? string.Empty,
        CalciumLevel = dto.CalciumLevel ?? string.Empty,
        RecorderId = dto.RecorderId, RecorderName = dto.RecorderName ?? string.Empty,
        RecordTime = ParseDate(dto.RecordTime) ?? DateTime.Now,
        Source = dto.Source ?? "manual", DeviceSn = dto.DeviceSn,
        Status = dto.Status ?? "pending",
        ReviewerId = dto.ReviewerId, ReviewerName = dto.ReviewerName,
        ReviewTime = ParseDate(dto.ReviewTime), ReviewRemark = dto.ReviewRemark, Remark = dto.Remark,
    };
}
