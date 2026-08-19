using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 学生健康档案服务实现
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
        return await GetVisionHistoryAsync(studentId).ContinueWith(t => t.Result.FirstOrDefault());
    }

    public async Task<VisionRecordDto> CreateVisionRecordAsync(VisionRecordDto dto)
    {
        var entity = VisionToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        _context.VisionRecords.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteVisionRecordAsync(long id)
    {
        var e = await _context.VisionRecords.FindAsync(id);
        if (e == null) return false;
        _context.VisionRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 口腔
    // ============================================================
    public async Task<List<OralRecordDto>> GetOralHistoryAsync(long studentId)
    {
        return await _context.OralRecords
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate)
            .ThenByDescending(r => r.Id)
            .Select(r => OralToDto(r))
            .ToListAsync();
    }

    public async Task<OralRecordDto?> GetLatestOralAsync(long studentId)
    {
        return (await GetOralHistoryAsync(studentId)).FirstOrDefault();
    }

    public async Task<OralRecordDto> CreateOralRecordAsync(OralRecordDto dto)
    {
        var entity = OralToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        _context.OralRecords.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteOralRecordAsync(long id)
    {
        var e = await _context.OralRecords.FindAsync(id);
        if (e == null) return false;
        _context.OralRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 心理
    // ============================================================
    public async Task<List<MentalRecordDto>> GetMentalHistoryAsync(long studentId)
    {
        return await _context.MentalRecords
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate)
            .ThenByDescending(r => r.Id)
            .Select(r => MentalToDto(r))
            .ToListAsync();
    }

    public async Task<MentalRecordDto?> GetLatestMentalAsync(long studentId)
    {
        return (await GetMentalHistoryAsync(studentId)).FirstOrDefault();
    }

    public async Task<MentalRecordDto> CreateMentalRecordAsync(MentalRecordDto dto)
    {
        var entity = MentalToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        _context.MentalRecords.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteMentalRecordAsync(long id)
    {
        var e = await _context.MentalRecords.FindAsync(id);
        if (e == null) return false;
        _context.MentalRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 体重
    // ============================================================
    public async Task<List<WeightRecordDto>> GetWeightHistoryAsync(long studentId)
    {
        return await _context.WeightRecords
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate)
            .ThenByDescending(r => r.Id)
            .Select(r => WeightToDto(r))
            .ToListAsync();
    }

    public async Task<WeightRecordDto?> GetLatestWeightAsync(long studentId)
    {
        return (await GetWeightHistoryAsync(studentId)).FirstOrDefault();
    }

    public async Task<WeightRecordDto> CreateWeightRecordAsync(WeightRecordDto dto)
    {
        var entity = WeightToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        _context.WeightRecords.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteWeightRecordAsync(long id)
    {
        var e = await _context.WeightRecords.FindAsync(id);
        if (e == null) return false;
        _context.WeightRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 骨骼
    // ============================================================
    public async Task<List<BoneRecordDto>> GetBoneHistoryAsync(long studentId)
    {
        return await _context.BoneRecords
            .AsNoTracking()
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.CheckDate)
            .ThenByDescending(r => r.Id)
            .Select(r => BoneToDto(r))
            .ToListAsync();
    }

    public async Task<BoneRecordDto?> GetLatestBoneAsync(long studentId)
    {
        return (await GetBoneHistoryAsync(studentId)).FirstOrDefault();
    }

    public async Task<BoneRecordDto> CreateBoneRecordAsync(BoneRecordDto dto)
    {
        var entity = BoneToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        _context.BoneRecords.Add(entity);
        await _context.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteBoneRecordAsync(long id)
    {
        var e = await _context.BoneRecords.FindAsync(id);
        if (e == null) return false;
        _context.BoneRecords.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 映射辅助
    // ============================================================
    private static VisionRecordDto VisionToDto(VisionRecord r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        LeftEye = r.LeftEye,
        RightEye = r.RightEye,
        VisionLevel = r.VisionLevel,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static VisionRecord VisionToEntity(VisionRecordDto dto) => new()
    {
        Id = dto.Id,
        StudentId = dto.StudentId,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        LeftEye = dto.LeftEye ?? string.Empty,
        RightEye = dto.RightEye ?? string.Empty,
        VisionLevel = dto.VisionLevel ?? string.Empty,
    };

    private static OralRecordDto OralToDto(OralRecord r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        ToothStatus = r.ToothStatus,
        CavityCount = r.CavityCount,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static OralRecord OralToEntity(OralRecordDto dto) => new()
    {
        Id = dto.Id,
        StudentId = dto.StudentId,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        ToothStatus = dto.ToothStatus ?? string.Empty,
        CavityCount = dto.CavityCount,
    };

    private static MentalRecordDto MentalToDto(MentalRecord r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        StressLevel = r.StressLevel,
        SleepQuality = r.SleepQuality,
        MoodStatus = r.MoodStatus,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static MentalRecord MentalToEntity(MentalRecordDto dto) => new()
    {
        Id = dto.Id,
        StudentId = dto.StudentId,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        StressLevel = dto.StressLevel ?? string.Empty,
        SleepQuality = dto.SleepQuality ?? string.Empty,
        MoodStatus = dto.MoodStatus ?? string.Empty,
    };

    private static WeightRecordDto WeightToDto(WeightRecord r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        Height = r.Height,
        Weight = r.Weight,
        Bmi = r.Bmi,
        BmiLevel = r.BmiLevel,
        WaistCircumference = r.WaistCircumference,
        HipCircumference = r.HipCircumference,
        Whr = r.Whr,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static WeightRecord WeightToEntity(WeightRecordDto dto) => new()
    {
        Id = dto.Id,
        StudentId = dto.StudentId,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        Height = dto.Height ?? string.Empty,
        Weight = dto.Weight ?? string.Empty,
        Bmi = dto.Bmi ?? string.Empty,
        BmiLevel = dto.BmiLevel ?? string.Empty,
        WaistCircumference = dto.WaistCircumference ?? string.Empty,
        HipCircumference = dto.HipCircumference ?? string.Empty,
        Whr = dto.Whr ?? string.Empty,
    };

    private static BoneRecordDto BoneToDto(BoneRecord r) => new()
    {
        Id = r.Id,
        StudentId = r.StudentId,
        CheckDate = r.CheckDate.ToString("yyyy-MM-dd"),
        BoneDensity = r.BoneDensity,
        BoneAge = r.BoneAge,
        VitaminD = r.VitaminD,
        CalciumLevel = r.CalciumLevel,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static BoneRecord BoneToEntity(BoneRecordDto dto) => new()
    {
        Id = dto.Id,
        StudentId = dto.StudentId,
        CheckDate = ParseDate(dto.CheckDate) ?? DateTime.Now,
        BoneDensity = dto.BoneDensity ?? string.Empty,
        BoneAge = dto.BoneAge ?? string.Empty,
        VitaminD = dto.VitaminD ?? string.Empty,
        CalciumLevel = dto.CalciumLevel ?? string.Empty,
    };

    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, out var d)) return d;
        return null;
    }
}
