using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 好友關係。
/// </summary>
public partial class FriendList
{
    /// <summary>
    /// 識別碼。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 好友關係狀態。
    /// </summary>
    public bool FriendRelations { get; set; }

    /// <summary>
    /// 好友邀請者識別碼。
    /// </summary>
    public int RequesterUserId { get; set; }

    /// <summary>
    /// 好友邀請接受者識別碼。
    /// </summary>
    public int AcceptorUserId { get; set; }

    /// <summary>
    /// 好友邀請接受者。
    /// </summary>
    public virtual User AcceptorUser { get; set; } = null!;

    /// <summary>
    /// 好友邀請者。
    /// </summary>
    public virtual User RequesterUser { get; set; } = null!;
}
