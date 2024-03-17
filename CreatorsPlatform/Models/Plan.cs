using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Plan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public int PlanPrice { get; set; }

    public string? Description { get; set; }

    public int? PlanLevel { get; set; }

    public DateOnly? PlanDate { get; set; }

    public int CreatorId { get; set; }

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual Creator Creator { get; set; } = null!;

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
