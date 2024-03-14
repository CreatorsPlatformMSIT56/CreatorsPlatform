using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class EventImage
{
    public int EventImageId { get; set; }

    public byte[]? ImageUrl { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
