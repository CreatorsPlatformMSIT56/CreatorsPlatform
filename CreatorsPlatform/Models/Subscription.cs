using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Subscription
{
    public int SubscriptionId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? PaymentMade { get; set; }

    public int? CategoryId { get; set; }

    public int CreatorId { get; set; }

    public int UserId { get; set; }

    public int PlanId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Creator Creator { get; set; } = null!;

    public virtual Plan Plan { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
