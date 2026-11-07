using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 訂閱方案。
/// </summary>
public partial class Plan
{
    /// <summary>
    /// 方案識別碼。
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// 方案名稱。
    /// </summary>
    public string PlanName { get; set; } = null!;

    /// <summary>
    /// 方案價格。
    /// </summary>
    public int PlanPrice { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 方案等級。
    /// </summary>
    public int? PlanLevel { get; set; }

    /// <summary>
    /// 方案日期。
    /// </summary>
    public DateOnly? PlanDate { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 相關的作品集合。
    /// </summary>
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的訂閱紀錄集合。
    /// </summary>
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
