using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 活動及其圖片的整合資料。
/// </summary>
public partial class EventsAndImage
{
    /// <summary>
    /// 活動圖片資料識別碼。
    /// </summary>
    public long? EvtImgId { get; set; }

    /// <summary>
    /// 活動識別碼。
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// 活動名稱。
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <summary>
    /// 活動描述。
    /// </summary>
    public string EventDes { get; set; } = null!;

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
    /// 活動是否取消。
    /// </summary>
    public bool? EventCancel { get; set; }

    /// <summary>
    /// 活動分類識別碼。
    /// </summary>
    public int EveCategory { get; set; }

    /// <summary>
    /// 活動創作者識別碼。
    /// </summary>
    public int? EveCreator { get; set; }

    /// <summary>
    /// 活動創作者名稱。
    /// </summary>
    public string? EveCreName { get; set; }

    public byte[]? EveCreAvatar { get; set; }

    /// <summary>
    /// 活動圖片識別碼。
    /// </summary>
    public int? EventImageId { get; set; }

    /// <summary>
    /// 圖片資料。
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// 圖片描述。
    /// </summary>
    public string? ImgDes { get; set; }

    /// <summary>
    /// 圖片標題。
    /// </summary>
    public string? ImgTitle { get; set; }

    /// <summary>
    /// 圖片創作者識別碼。
    /// </summary>
    public int? ImgCreator { get; set; }

    /// <summary>
    /// 是否為範例圖片。
    /// </summary>
    public bool? ImageSample { get; set; }

    /// <summary>
    /// 活動圖片是否取消。
    /// </summary>
    public bool? EveImgCancel { get; set; }

    /// <summary>
    /// 活動圖片喜歡次數。
    /// </summary>
    public int? EvePostLike { get; set; }

    public byte[]? ImgCreAvatar { get; set; }

    /// <summary>
    /// 圖片創作者名稱。
    /// </summary>
    public string? ImgCreName { get; set; }
}
