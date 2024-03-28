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

    public string? Banner { get; set; }

    public bool? EventCancel { get; set; }

    public int EveCategory { get; set; }

    public int? EveCreator { get; set; }

    public string? EveCreName { get; set; }

    public byte[]? EveCreAvatar { get; set; }

    public int? EventImageId { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImgDes { get; set; }

    public string? ImgTitle { get; set; }

    public int? ImgCreator { get; set; }

    public bool? ImageSample { get; set; }

    public bool? EveImgCancel { get; set; }

    public int? EvePostLike { get; set; }

    public byte[]? ImgCreAvatar { get; set; }

    public string? ImgCreName { get; set; }
}
