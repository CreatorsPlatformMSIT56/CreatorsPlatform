using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 使用者。
/// </summary>
public partial class User
{
    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 使用者名稱。
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 電子郵件。
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 密碼。
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 註冊時間。
    /// </summary>
    public DateTime? RegisterDate { get; set; }

    /// <summary>
    /// 最後登入時間。
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    public byte[]? Avatar { get; set; }

    /// <summary>
    /// 生日。
    /// </summary>
    public DateOnly? BirthdayDate { get; set; }

    /// <summary>
    /// 點數。
    /// </summary>
    public int? Point { get; set; }

    /// <summary>
    /// 電子郵件驗證狀態。
    /// </summary>
    public bool? EmailCertification { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int? CreatorId { get; set; }

    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// 相關的作品分類。
    /// </summary>
    public virtual Category? Category { get; set; }

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
    /// 相關的委託訂單集合。
    /// </summary>
    public virtual ICollection<CommissionOrder> CommissionOrders { get; set; } =
        new List<CommissionOrder>();

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator? Creator { get; set; }

    /// <summary>
    /// 相關的追蹤關係集合。
    /// </summary>
    public virtual ICollection<Follow> Follows { get; set; } = new List<Follow>();

    /// <summary>
    /// 以此使用者為接受者的好友關係。
    /// </summary>
    public virtual ICollection<FriendList> FriendListAcceptorUsers { get; set; } =
        new List<FriendList>();

    /// <summary>
    /// 以此使用者為邀請者的好友關係。
    /// </summary>
    public virtual ICollection<FriendList> FriendListRequesterUsers { get; set; } =
        new List<FriendList>();

    /// <summary>
    /// 相關的訂閱紀錄集合。
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
