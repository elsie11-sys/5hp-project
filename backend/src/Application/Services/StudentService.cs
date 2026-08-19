using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

/// <summary>
/// 学生档案服务实现
/// </summary>
public class StudentService : IStudentService
{
    private readonly IApplicationDbContext _context;

    public StudentService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // 查询
    // ============================================================

    public async Task<List<StudentDto>> GetAllStudentsAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .OrderByDescending(s => s.Id)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<(List<StudentDto> Items, int Total)> GetPagedStudentsAsync(StudentQuery query)
    {
        var q = _context.Students.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.School))
        {
            q = q.Where(s => s.School == query.School);
        }

        if (!string.IsNullOrWhiteSpace(query.ClassName))
        {
            q = q.Where(s => s.ClassName == query.ClassName);
        }

        if (!string.IsNullOrWhiteSpace(query.StudentNo))
        {
            q = q.Where(s => s.StudentNo == query.StudentNo);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            q = q.Where(s => s.Name.Contains(query.Name));
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(s => s.Name.Contains(query.Keyword) || s.StudentNo.Contains(query.Keyword));
        }

        if (query.Intervention.HasValue)
        {
            q = q.Where(s => s.Intervention == query.Intervention.Value);
        }

        var total = await q.CountAsync();

        var items = await q
            .OrderByDescending(s => s.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => MapToDto(s))
            .ToListAsync();

        return (items, total);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(long id)
    {
        var s = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return s == null ? null : MapToDto(s);
    }

    public async Task<StudentDto?> GetStudentByNoAsync(string studentNo)
    {
        if (string.IsNullOrWhiteSpace(studentNo)) return null;
        var s = await _context.Students.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StudentNo == studentNo);
        return s == null ? null : MapToDto(s);
    }

    public async Task<bool> IsStudentNoExistsAsync(string studentNo, long? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(studentNo)) return false;
        var q = _context.Students.AsNoTracking().Where(s => s.StudentNo == studentNo);
        if (excludeId.HasValue)
        {
            q = q.Where(s => s.Id != excludeId.Value);
        }
        return await q.AnyAsync();
    }

    // ============================================================
    // 命令
    // ============================================================

    public async Task<StudentDto> CreateStudentAsync(StudentDto dto)
    {
        var entity = MapToEntity(dto);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;

        _context.Students.Add(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        dto.CreatedAt = entity.CreatedAt;
        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<StudentDto> UpdateStudentAsync(StudentDto dto)
    {
        var entity = await _context.Students.FindAsync(dto.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"找不到 ID 为 {dto.Id} 的学生档案");
        }

        entity.StudentNo = dto.StudentNo;
        entity.Avatar = dto.Avatar;
        entity.GradeYear = dto.GradeYear;
        entity.School = dto.School;
        entity.ClassName = dto.ClassName;
        entity.Name = dto.Name;
        entity.Gender = dto.Gender;
        entity.Nation = dto.Nation;
        entity.Birthday = ParseDate(dto.Birthday);
        entity.IdCard = dto.IdCard;
        entity.NativePlace = dto.NativePlace;
        entity.Address = dto.Address;
        entity.ParentPhone = dto.ParentPhone;
        entity.ParentContact = dto.ParentContact;
        entity.AllergyHistory = dto.AllergyHistory;
        entity.FatherMyopia = dto.FatherMyopia;
        entity.MotherMyopia = dto.MotherMyopia;
        entity.DataSource = dto.DataSource;
        entity.Intervention = dto.Intervention;
        entity.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        dto.UpdatedAt = entity.UpdatedAt;
        return dto;
    }

    public async Task<bool> DeleteStudentAsync(long id)
    {
        var s = await _context.Students.FindAsync(id);
        if (s == null) return false;
        _context.Students.Remove(s);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> BatchDeleteStudentsAsync(IReadOnlyList<long> ids)
    {
        if (ids == null || ids.Count == 0) return 0;
        var rows = await _context.Students.Where(s => ids.Contains(s.Id)).ToListAsync();
        if (rows.Count == 0) return 0;
        _context.Students.RemoveRange(rows);
        await _context.SaveChangesAsync();
        return rows.Count;
    }

    public async Task<bool> ToggleInterventionAsync(long id, bool intervention)
    {
        var s = await _context.Students.FindAsync(id);
        if (s == null) return false;
        s.Intervention = intervention;
        s.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================
    // 映射辅助
    // ============================================================

    private static StudentDto MapToDto(Student s) => new()
    {
        Id = s.Id,
        StudentNo = s.StudentNo,
        Avatar = s.Avatar,
        GradeYear = s.GradeYear,
        School = s.School,
        ClassName = s.ClassName,
        Name = s.Name,
        Gender = s.Gender,
        Nation = s.Nation,
        Birthday = s.Birthday?.ToString("yyyy-MM-dd"),
        IdCard = s.IdCard,
        NativePlace = s.NativePlace,
        Address = s.Address,
        ParentPhone = s.ParentPhone,
        ParentContact = s.ParentContact,
        AllergyHistory = s.AllergyHistory,
        FatherMyopia = s.FatherMyopia,
        MotherMyopia = s.MotherMyopia,
        DataSource = s.DataSource,
        Intervention = s.Intervention,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt,
    };

    private static Student MapToEntity(StudentDto dto) => new()
    {
        StudentNo = dto.StudentNo ?? string.Empty,
        Avatar = string.IsNullOrWhiteSpace(dto.Avatar) ? "👦" : dto.Avatar,
        GradeYear = dto.GradeYear ?? string.Empty,
        School = dto.School ?? string.Empty,
        ClassName = dto.ClassName ?? string.Empty,
        Name = dto.Name ?? string.Empty,
        Gender = string.IsNullOrWhiteSpace(dto.Gender) ? "男" : dto.Gender,
        Nation = string.IsNullOrWhiteSpace(dto.Nation) ? "汉族" : dto.Nation,
        Birthday = ParseDate(dto.Birthday),
        IdCard = dto.IdCard ?? string.Empty,
        NativePlace = dto.NativePlace ?? string.Empty,
        Address = dto.Address ?? string.Empty,
        ParentPhone = dto.ParentPhone ?? string.Empty,
        ParentContact = dto.ParentContact ?? string.Empty,
        AllergyHistory = string.IsNullOrWhiteSpace(dto.AllergyHistory) ? "未知" : dto.AllergyHistory,
        FatherMyopia = string.IsNullOrWhiteSpace(dto.FatherMyopia) ? "未知" : dto.FatherMyopia,
        MotherMyopia = string.IsNullOrWhiteSpace(dto.MotherMyopia) ? "未知" : dto.MotherMyopia,
        DataSource = string.IsNullOrWhiteSpace(dto.DataSource) ? "手动录入" : dto.DataSource,
        Intervention = dto.Intervention,
    };

    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateTime.TryParse(s, out var d)) return d;
        return null;
    }
}
