using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 留言回覆。
/// </summary>
public partial class CommentResponse
{
    /// <summary>
    /// 留言回覆識別碼。
    /// </summary>
    public int CommentResponseId { get; set; }

    /// <summary>
    /// 回覆內容。
    /// </summary>
    public string Response { get; set; } = null!;

    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 作品識別碼。
    /// </summary>
    public int ContentId { get; set; }

    /// <summary>
    /// 留言識別碼。
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// 相關的作品留言。
    /// </summary>
    public virtual Comment Comment { get; set; } = null!;

    /// <summary>
    /// 相關的作品。
    /// </summary>
    public virtual Content Content { get; set; } = null!;

    /// <summary>
    /// 相關的使用者。
    /// </summary>
    public virtual User User { get; set; } = null!;
}
