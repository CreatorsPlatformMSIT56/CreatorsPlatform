using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Subtitle> Subtitles { get; set; } = new List<Subtitle>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
