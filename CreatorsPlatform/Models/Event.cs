using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 活動。
/// </summary>
public partial class Event
{
    /// <summary>
    /// 活動識別碼。
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// 活動名稱。
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <summary>
    /// 開始日期。
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期。
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 活動橫幅。
    /// </summary>
    public string? Banner { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 活動樣式。
    /// </summary>
    public string? EventStyle { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 活動描述字串。
    /// </summary>
    public string DescriptionString { get; set; } = null!;

    /// <summary>
    /// 活動是否取消。
    /// </summary>
    public bool? EventCancel { get; set; }

    /// <summary>
    /// 相關的作品分類。
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的活動圖片或投稿集合。
    /// </summary>
    public virtual ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();
}
