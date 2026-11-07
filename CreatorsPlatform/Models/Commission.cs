using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 委託項目。
/// </summary>
public partial class Commission
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
    /// 相關的委託圖片集合。
    /// </summary>
    public virtual ICollection<CommissionImage> CommissionImages { get; set; } =
        new List<CommissionImage>();

    /// <summary>
    /// 相關的委託訂單集合。
    /// </summary>
    public virtual ICollection<CommissionOrder> CommissionOrders { get; set; } =
        new List<CommissionOrder>();

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的子分類。
    /// </summary>
    public virtual Subtitle Subtitle { get; set; } = null!;
}
