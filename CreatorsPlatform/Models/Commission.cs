using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Commission
{
    public int CommissionId { get; set; }

    public string? Title { get; set; }

    public int PriceMin { get; set; }

    public int? PriceMax { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly PutUpDate { get; set; }

    public DateOnly? OverDate { get; set; }

    public int CreatorId { get; set; }

    public int UserId { get; set; }
    public int SubtitleId { get; set; }

    public virtual ICollection<CommissionImage> CommissionImages { get; set; } = new List<CommissionImage>();

    public virtual ICollection<CommissionOrder> CommissionOrders { get; set; } = new List<CommissionOrder>();

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual Creator Creator { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
