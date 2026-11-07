using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 作品分類。
/// </summary>
public partial class Category
{
    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 分類名稱。
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    /// <summary>
    /// 相關的活動集合。
    /// </summary>
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    /// <summary>
    /// 相關的訂閱紀錄集合。
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    /// <summary>
    /// 相關的子分類集合。
    /// </summary>
    public virtual ICollection<Subtitle> Subtitles { get; set; } = new List<Subtitle>();

    /// <summary>
    /// 相關的使用者集合。
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
