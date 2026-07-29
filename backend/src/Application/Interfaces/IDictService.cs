using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IDictService
{
    /// <summary>分页查询字典</summary>
    Task<(List<DictDto> Items, int Total)> GetPagedAsync(DictQuery query);

    /// <summary>根据 ID 获取字典</summary>
    Task<DictDto?> GetByIdAsync(long id);

    /// <summary>根据 DictType 查单个（业务代码 → 字典）</summary>
    Task<DictDto?> GetByTypeAsync(string dictType);

    /// <summary>全量（用于下拉 / 缓存刷新）</summary>
    Task<List<DictDto>> GetAllAsync();

    /// <summary>新建字典</summary>
    Task<DictDto> CreateAsync(DictDto dto);

    /// <summary>更新字典</summary>
    Task<DictDto> UpdateAsync(DictDto dto);

    /// <summary>删除单个</summary>
    Task<bool> DeleteAsync(long id);

    /// <summary>批量删除</summary>
    Task<int> BatchDeleteAsync(IReadOnlyList<long> ids);

    /// <summary>检查 DictType 是否已存在（创建/编辑时校验）</summary>
    Task<bool> ExistsTypeAsync(string dictType, long? excludeId = null);

    /// <summary>导出当前查询结果（返回数据，由 API 层生成 Excel）</summary>
    Task<List<DictDto>> ExportAsync(DictQuery query);
}
