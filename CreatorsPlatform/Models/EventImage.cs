using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class EventImage
{
    public int EventImageId { get; set; }

    public byte[]? ImageUrl { get; set; }

    public int EventId { get; set; }

    public bool? ImageSample { get; set; }

    public int CreatorId { get; set; }

    public virtual Creator Creator { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
