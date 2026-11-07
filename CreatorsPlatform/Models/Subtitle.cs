using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 子分類。
/// </summary>
public partial class Subtitle
{
    /// <summary>
    /// 子分類識別碼。
    /// </summary>
    public int SubtitleId { get; set; }

    /// <summary>
    /// 子分類名稱。
    /// </summary>
    public string SubtitleName { get; set; } = null!;

    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 相關的作品分類。
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 相關的委託項目集合。
    /// </summary>
    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
}
