using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class CommissionWithImageAndWord
{
    public int CommissionId { get; set; }

    public string? Title { get; set; }

    public int PriceMin { get; set; }

    public int? PriceMax { get; set; }

    public string Description { get; set; } = null!;

    public DateOnly PutUpDate { get; set; }

    public DateOnly? OverDate { get; set; }

    public int CreatorId { get; set; }

    public int SubtitleId { get; set; }

    public string? ImageUrl { get; set; }

    public string? SubtitleName { get; set; }
}
