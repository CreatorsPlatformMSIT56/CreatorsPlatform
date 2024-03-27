using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class EventImage
{
    public int EventImageId { get; set; }

    public string? ImageUrl { get; set; }

    public int EventId { get; set; }

    public bool? ImageSample { get; set; }

    public int CreatorId { get; set; }

    public string? Description { get; set; }

    public string? ImageTitle { get; set; }

    public int? EvePostLike { get; set; }

    public bool? EveImgCancel { get; set; }

    public virtual Creator Creator { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
