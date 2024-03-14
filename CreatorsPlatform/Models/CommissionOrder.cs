using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class CommissionOrder
{
    public int CommissionOrderId { get; set; }

    public string? Title { get; set; }

    public int Price { get; set; }

    public DateOnly OrderDate { get; set; }

    public DateOnly? DeadlineDate { get; set; }

    public string? WorkStatus { get; set; }

    public string? Description { get; set; }

    public int CommissionId { get; set; }

    public virtual Commission Commission { get; set; } = null!;
}
