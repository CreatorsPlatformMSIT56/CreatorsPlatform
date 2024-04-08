using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Follow
{
    public int FollowId { get; set; }

    public int UserId { get; set; }

    public int CreatorId { get; set; }

    public bool Unfollow { get; set; }

    public virtual Creator Creator { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
