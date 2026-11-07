using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 作品留言。
/// </summary>
public partial class Comment
{
    /// <summary>
    /// 留言識別碼。
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// 留言內容。
    /// </summary>
    public string Comment1 { get; set; } = null!;

    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 作品識別碼。
    /// </summary>
    public int ContentId { get; set; }

    /// <summary>
    /// 相關的留言回覆集合。
    /// </summary>
    public virtual ICollection<CommentResponse> CommentResponses { get; set; } =
        new List<CommentResponse>();

    /// <summary>
    /// 相關的作品。
    /// </summary>
    public virtual Content Content { get; set; } = null!;

    /// <summary>
    /// 相關的使用者。
    /// </summary>
    public virtual User User { get; set; } = null!;
}
