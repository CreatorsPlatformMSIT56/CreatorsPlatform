using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 活動圖片或投稿。
/// </summary>
public partial class EventImage
{
    /// <summary>
    /// 活動圖片識別碼。
    /// </summary>
    public int EventImageId { get; set; }

    /// <summary>
    /// 圖片資料。
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// 活動識別碼。
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// 是否為範例圖片。
    /// </summary>
    public bool ImageSample { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 圖片標題。
    /// </summary>
    public string? ImageTitle { get; set; }

    /// <summary>
    /// 活動圖片喜歡次數。
    /// </summary>
    public int? EvePostLike { get; set; }

    /// <summary>
    /// 活動圖片是否取消。
    /// </summary>
    public bool? EveImgCancel { get; set; }

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的活動。
    /// </summary>
    public virtual Event Event { get; set; } = null!;
}
