using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
}
