using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 訂閱紀錄。
/// </summary>
public partial class Subscription
{
    /// <summary>
    /// 訂閱識別碼。
    /// </summary>
    public int SubscriptionId { get; set; }

    /// <summary>
    /// 開始日期。
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// 結束日期。
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// 付款狀態。
    /// </summary>
    public bool? PaymentMade { get; set; }

    /// <summary>
    /// 分類識別碼。
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// 創作者識別碼。
    /// </summary>
    public int CreatorId { get; set; }

    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 方案識別碼。
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// 相關的作品分類。
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// 相關的創作者。
    /// </summary>
    public virtual Creator Creator { get; set; } = null!;

    /// <summary>
    /// 相關的訂閱方案。
    /// </summary>
    public virtual Plan Plan { get; set; } = null!;

    /// <summary>
    /// 相關的使用者。
    /// </summary>
    public virtual User User { get; set; } = null!;
}
