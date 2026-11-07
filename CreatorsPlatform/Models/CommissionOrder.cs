using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

/// <summary>
/// 委託訂單。
/// </summary>
public partial class CommissionOrder
{
    /// <summary>
    /// 委託訂單識別碼。
    /// </summary>
    public int CommissionOrderId { get; set; }

    /// <summary>
    /// 標題。
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 價格。
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 訂單日期。
    /// </summary>
    public DateOnly OrderDate { get; set; }

    /// <summary>
    /// 交件期限。
    /// </summary>
    public DateOnly? DeadlineDate { get; set; }

    /// <summary>
    /// 委託工作狀態。
    /// </summary>
    public string? WorkStatus { get; set; }

    /// <summary>
    /// 描述內容。
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 委託識別碼。
    /// </summary>
    public int CommissionId { get; set; }

    /// <summary>
    /// 使用者識別碼。
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 相關的委託項目。
    /// </summary>
    public virtual Commission Commission { get; set; } = null!;

    /// <summary>
    /// 相關的使用者。
    /// </summary>
    public virtual User User { get; set; } = null!;
}
