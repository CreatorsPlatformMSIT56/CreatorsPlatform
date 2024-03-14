using System;
using System.Collections.Generic;

namespace CreatorsPlatform.Models;

public partial class CommentResponse
{
    public int CommentResponseId { get; set; }

    public string Response { get; set; } = null!;

    public int UserId { get; set; }

    public int ContentId { get; set; }

    public int CommentId { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual Content Content { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
