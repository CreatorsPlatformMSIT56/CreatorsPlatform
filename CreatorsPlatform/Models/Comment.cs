using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string Comment1 { get; set; } = null!;

    public int UserId { get; set; }

    public int ContentId { get; set; }

    public virtual ICollection<CommentResponse> CommentResponses { get; set; } = new List<CommentResponse>();

    public virtual Content Content { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
