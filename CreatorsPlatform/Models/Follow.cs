using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 追蹤關係。
/// </summary>
public partial class Follow
{
    /// <summary>
    /// 追蹤識別碼。
    /// </summary>
    public int FollowId { get; set; }

    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 是否取消追蹤。
    /// </summary>
    public bool Unfollow { get; set; }

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的使用者。
    /// </summary>
    public virtual User User { get; set; } = null!;
}
