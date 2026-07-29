namespace Application.DTOs;

/// <summary>
/// 批量导入结果汇总
/// </summary>
public class ImportResult
{
    /// <summary>Excel 中总数据行数（不含表头）</summary>
    public int TotalRows { get; set; }

    /// <summary>成功插入的数量</summary>
    public int SuccessCount { get; set; }

    /// <summary>失败 / 跳过的数量</summary>
    public int FailedCount { get; set; }

    /// <summary>每条失败的原因（带 Excel 行号）</summary>
    public List<ImportError> Errors { get; set; } = new();
}

public class ImportError
{
    /// <summary>Excel 中的行号（2 = 第二行，表头算第 1 行）</summary>
    public int Row { get; set; }

    /// <summary>失败原因</summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 批量删除请求
/// </summary>
public class BatchDeleteRequest
{
    public List<long> Ids { get; set; } = new();
}
