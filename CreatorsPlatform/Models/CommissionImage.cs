using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class CommissionImage
{
    public int CommissionImageId { get; set; }

    public string? ImageUrl { get; set; }

    public int CommissionId { get; set; }

    public virtual Commission Commission { get; set; } = null!;
}
