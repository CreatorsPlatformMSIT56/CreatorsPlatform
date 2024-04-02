using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class ContentTag
{
    public int ContentTagId { get; set; }

    public int? ContentId { get; set; }

    public int? TagId { get; set; }

    public virtual Content? Content { get; set; }

    public virtual Tag? Tag { get; set; }
}
