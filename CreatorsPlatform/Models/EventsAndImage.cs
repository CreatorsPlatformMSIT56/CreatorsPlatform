using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class EventsAndImage
{
    public long? EvtImgId { get; set; }

    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string EventDes { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? EventCreator { get; set; }

    public int? EventImageId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageDes { get; set; }

    public int? ImageCreator { get; set; }

    public bool? ImageSample { get; set; }
}
