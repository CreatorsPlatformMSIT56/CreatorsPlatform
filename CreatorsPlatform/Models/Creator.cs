﻿using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Creator
{
    public int CreatorId { get; set; }

    public string? Description { get; set; }

    public string Notice { get; set; } = null!;

    public string? BannerUrl { get; set; }

    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual ICollection<EventImage> EventImages { get; set; } = new List<EventImage>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Follow> Follows { get; set; } = new List<Follow>();

    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
