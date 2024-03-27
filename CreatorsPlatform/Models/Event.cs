using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Banner { get; set; }

    public string Description { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? EventStyle { get; set; }

    public int? CreatorId { get; set; }

    public string? DescriptionString { get; set; }

    public bool? EventCancel { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual Creator? Creator { get; set; }

    public virtual ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();
}
