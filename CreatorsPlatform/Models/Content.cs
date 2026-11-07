using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 作品。
/// </summary>
public partial class Content
{
    /// <summary>
    /// 作品識別碼。
    /// </summary>
    public int ContentId { get; set; }

    /// <summary>
    /// 標題。
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// 上傳時間。
    /// </summary>
    public DateTime? UploadDate { get; set; }

    /// <summary>
    /// 下架時間。
    /// </summary>
    public DateTime? PullOffDate { get; set; }

    public byte[]? ImageUrl { get; set; }

    /// <summary>
    /// 喜歡次數。
    /// </summary>
    public int? Likes { get; set; }

    /// <summary>
    /// 庫存數量。
    /// </summary>
    public int? NumsInStock { get; set; }

    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 子分類識別碼。
    /// </summary>
    public int? SubtitleId { get; set; }

    /// <summary>
    /// 標籤識別碼。
    /// </summary>
    public int? TagId { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 方案識別碼。
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// 活動識別碼。
    /// </summary>
    public int? EventId { get; set; }

    /// <summary>
    /// 是否為活動範例。
    /// </summary>
    public bool? EventSample { get; set; }

    /// <summary>
    /// 委託識別碼。
    /// </summary>
    public int? CommissionId { get; set; }

    /// <summary>
    /// 是否為委託範例。
    /// </summary>
    public bool? CommissionSample { get; set; }

    /// <summary>
    /// 作品價格。
    /// </summary>
    public int? ContentPrice { get; set; }

    /// <summary>
    /// 相關的作品分類。
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 相關的留言回覆集合。
    /// </summary>
    public virtual ICollection<CommentResponse> CommentResponses { get; set; } =
        new List<CommentResponse>();

    /// <summary>
    /// 相關的作品留言集合。
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    /// <summary>
    /// 相關的委託項目。
    /// </summary>
    public virtual Commission? Commission { get; set; }

    /// <summary>
    /// 相關的作品與標籤的關聯集合。
    /// </summary>
    public virtual ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的活動。
    /// </summary>
    public virtual Event? Event { get; set; }

    /// <summary>
    /// 相關的訂閱方案。
    /// </summary>
    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// 相關的子分類。
    /// </summary>
    public virtual Subtitle? Subtitle { get; set; }

    /// <summary>
    /// 相關的標籤。
    /// </summary>
    public virtual Tag? Tag { get; set; }
}
