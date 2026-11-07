using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 創作者。
/// </summary>
public partial class Creator
{
    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 公告內容。
    /// </summary>
    public string Notice { get; set; } = null!;

    /// <summary>
    /// 橫幅網址。
    /// </summary>
    public string? BannerUrl { get; set; }

    /// <summary>
    /// 相關的委託項目集合。
    /// </summary>
    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    /// <summary>
    /// 相關的活動圖片或投稿集合。
    /// </summary>
    public virtual ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();

    /// <summary>
    /// 相關的活動集合。
    /// </summary>
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    /// <summary>
    /// 相關的追蹤關係集合。
    /// </summary>
    public virtual ICollection<Follow> Follows { get; set; } = new List<Follow>();

    /// <summary>
    /// 相關的訂閱方案集合。
    /// </summary>
    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

    /// <summary>
    /// 相關的訂閱紀錄集合。
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    /// <summary>
    /// 相關的使用者集合。
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
