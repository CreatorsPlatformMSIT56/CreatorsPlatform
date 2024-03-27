using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime RegisterDate { get; set; }

    public DateTime LastLoginDate { get; set; }

    public byte[]? Avatar { get; set; }

    public DateOnly? BirthdayDate { get; set; }

    public int? Point { get; set; }

    public bool? EmailCertification { get; set; }

    public int? CreatorId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<CommentResponse> CommentResponses { get; set; } = new List<CommentResponse>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<CommissionOrder> CommissionOrders { get; set; } = new List<CommissionOrder>();

    public virtual Creator? Creator { get; set; }

    public virtual ICollection<FriendList> FriendListAcceptorUsers { get; set; } = new List<FriendList>();

    public virtual ICollection<FriendList> FriendListRequesterUsers { get; set; } = new List<FriendList>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
