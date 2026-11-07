using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 標籤。
/// </summary>
public partial class Tag
{
    /// <summary>
    /// 標籤識別碼。
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// 標籤名稱。
    /// </summary>
    public string TagName { get; set; } = null!;

    /// <summary>
    /// 相關的作品與標籤的關聯集合。
    /// </summary>
    public virtual ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
}
