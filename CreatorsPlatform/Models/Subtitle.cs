using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Subtitle
{
    public int SubtitleId { get; set; }

    public string SubtitleName { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
