using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 委託圖片。
/// </summary>
public partial class CommissionImage
{
    /// <summary>
    /// 委託圖片識別碼。
    /// </summary>
    public int CommissionImageId { get; set; }

    /// <summary>
    /// 圖片資料。
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 委託識別碼。
    /// </summary>
    public int CommissionId { get; set; }

    /// <summary>
    /// 相關的委託項目。
    /// </summary>
    public virtual Commission Commission { get; set; } = null!;
}
