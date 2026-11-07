using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 包含圖片及分類名稱的委託資料。
/// </summary>
public partial class CommissionWithImageAndWord
{
    /// <summary>
    /// 委託識別碼。
    /// </summary>
    public int CommissionId { get; set; }

    /// <summary>
    /// 標題。
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 最低價格。
    /// </summary>
    public int PriceMin { get; set; }

    /// <summary>
    /// 最高價格。
    /// </summary>
    public int? PriceMax { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// 上架日期。
    /// </summary>
    public DateOnly PutUpDate { get; set; }

    /// <summary>
    /// 截止日期。
    /// </summary>
    public DateOnly? OverDate { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 子分類識別碼。
    /// </summary>
    public int SubtitleId { get; set; }

    /// <summary>
    /// 圖片資料。
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 子分類名稱。
    /// </summary>
    public string? SubtitleName { get; set; }
}
