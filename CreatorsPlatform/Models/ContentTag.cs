using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 作品與標籤的關聯。
/// </summary>
public partial class ContentTag
{
    /// <summary>
    /// 作品標籤關聯識別碼。
    /// </summary>
    public int ContentTagId { get; set; }

    /// <summary>
    /// 作品識別碼。
    /// </summary>
    public int? ContentId { get; set; }

    /// <summary>
    /// 標籤識別碼。
    /// </summary>
    public int? TagId { get; set; }

    /// <summary>
    /// 相關的作品。
    /// </summary>
    public virtual Content? Content { get; set; }

    /// <summary>
    /// 相關的標籤。
    /// </summary>
    public virtual Tag? Tag { get; set; }
}
