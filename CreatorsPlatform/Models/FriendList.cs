using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class FriendList
{
    public int Id { get; set; }

    public bool FriendRelations { get; set; }

    public int RequesterUserId { get; set; }

    public int AcceptorUserId { get; set; }

    public virtual User AcceptorUser { get; set; } = null!;

    public virtual User RequesterUser { get; set; } = null!;
}
