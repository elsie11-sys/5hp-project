using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 组织管理服务接口
/// </summary>
public interface IOrgService
{
    /// <summary>
    /// 分页查询组织列表
    /// </summary>
    Task<(List<OrgDto> Items, int Total)> GetPagedOrgsAsync(OrgQuery query);

    /// <summary>
    /// 获取所有组织（扁平列表）
    /// </summary>
    Task<List<OrgDto>> GetAllOrgsAsync();

    /// <summary>
    /// 获取组织树
    /// </summary>
    Task<List<OrgDto>> GetOrgTreeAsync();

    /// <summary>
    /// 根据ID获取单个组织
    /// </summary>
    Task<OrgDto?> GetOrgByIdAsync(long id);

    /// <summary>
    /// 创建组织
    /// </summary>
    Task<OrgDto> CreateOrgAsync(OrgDto orgDto);

    /// <summary>
    /// 更新组织
    /// </summary>
    Task<OrgDto> UpdateOrgAsync(OrgDto orgDto);

    /// <summary>
    /// 删除单个组织
    /// </summary>
    Task<bool> DeleteOrgAsync(long id);

    /// <summary>
    /// 批量删除组织
    /// </summary>
    Task<(int DeletedCount, string Message)> BatchDeleteOrgsAsync(IReadOnlyList<long> ids);
}
