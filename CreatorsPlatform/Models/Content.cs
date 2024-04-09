using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreatorsPlatform.Models;

public partial class Content
{
    [NotMapped]
    public string? ImageFile { get; set; }
    public int ContentId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? UploadDate { get; set; }

    public DateTime? PullOffDate { get; set; }
    // 為了拿到圖片檔案
    //[NotMapped]
    //public IFormFile? ImageFile { get; set; }
    public byte[]? ImageUrl { get; set; }

    public int? Likes { get; set; }

    public int? NumsInStock { get; set; }

    public int CategoryId { get; set; }

    public int? SubtitleId { get; set; }

    public int? TagId { get; set; }

    public int CreatorId { get; set; }

    public int PlanId { get; set; }

    public int? EventId { get; set; }

    public bool? EventSample { get; set; }

    public int? CommissionId { get; set; }

    public bool? CommissionSample { get; set; }

    public int? ContentPrice { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CommentResponse> CommentResponses { get; set; } = new List<CommentResponse>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Commission? Commission { get; set; }

    public virtual ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();

    public virtual Creator Creator { get; set; } = null!;

    public virtual Event? Event { get; set; }

    public virtual Plan Plan { get; set; } = null!;

    public virtual Subtitle? Subtitle { get; set; }

    public virtual Tag? Tag { get; set; }
}
