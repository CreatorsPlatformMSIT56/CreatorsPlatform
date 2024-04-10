using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Models;

public partial class ImaginkContext : DbContext
{
    public ImaginkContext()
    {
    }

    public ImaginkContext(DbContextOptions<ImaginkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentResponse> CommentResponses { get; set; }

    public virtual DbSet<Commission> Commissions { get; set; }

    public virtual DbSet<CommissionImage> CommissionImages { get; set; }

    public virtual DbSet<CommissionOrder> CommissionOrders { get; set; }

    public virtual DbSet<CommissionWithImageAndWord> CommissionWithImageAndWords { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentTag> ContentTags { get; set; }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventImage> EventImages { get; set; }

    public virtual DbSet<EventsAndImage> EventsAndImages { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<FriendList> FriendLists { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Subtitle> Subtitles { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=Imagink;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B66E41C26");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFAAD1800B14");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Comment1)
                .HasMaxLength(500)
                .HasColumnName("Comment");
            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Content).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__Conten__07C12930");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__UserID__06CD04F7");
        });

        modelBuilder.Entity<CommentResponse>(entity =>
        {
            entity.HasKey(e => e.CommentResponseId).HasName("PK__CommentR__41BC2FBF68EB0A06");

            entity.Property(e => e.CommentResponseId).HasColumnName("CommentResponseID");
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.Response).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentResponses)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentRe__Comme__0C85DE4D");

            entity.HasOne(d => d.Content).WithMany(p => p.CommentResponses)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentRe__Conte__0B91BA14");

            entity.HasOne(d => d.User).WithMany(p => p.CommentResponses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentRe__UserI__0A9D95DB");
        });

        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasKey(e => e.CommissionId).HasName("PK__Commissi__6C2C8CEC19A627C3");

            entity.Property(e => e.CommissionId).HasColumnName("CommissionID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.SubtitleId).HasColumnName("SubtitleID");
            entity.Property(e => e.Title).HasMaxLength(500);

            entity.HasOne(d => d.Creator).WithMany(p => p.Commissions)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commissio__Creat__5812160E");

            entity.HasOne(d => d.Subtitle).WithMany(p => p.Commissions)
                .HasForeignKey(d => d.SubtitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commissio__Subti__4D5F7D71");
        });

        modelBuilder.Entity<CommissionImage>(entity =>
        {
            entity.HasKey(e => e.CommissionImageId).HasName("PK__Commissi__9C714FFEB1239DCE");

            entity.Property(e => e.CommissionImageId).HasColumnName("CommissionImageID");
            entity.Property(e => e.CommissionId).HasColumnName("CommissionID");
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

            entity.HasOne(d => d.Commission).WithMany(p => p.CommissionImages)
                .HasForeignKey(d => d.CommissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commissio__Commi__19DFD96B");
        });

        modelBuilder.Entity<CommissionOrder>(entity =>
        {
            entity.HasKey(e => e.CommissionOrderId).HasName("PK__Commissi__F3359E7398ACB25F");

            entity.Property(e => e.CommissionOrderId).HasColumnName("CommissionOrderID");
            entity.Property(e => e.CommissionId).HasColumnName("CommissionID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WorkStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Commission).WithMany(p => p.CommissionOrders)
                .HasForeignKey(d => d.CommissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commissio__Commi__59FA5E80");

            entity.HasOne(d => d.User).WithMany(p => p.CommissionOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commissio__UserI__19AACF41");
        });

        modelBuilder.Entity<CommissionWithImageAndWord>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CommissionWithImageAndWords");

            entity.Property(e => e.CommissionId).HasColumnName("CommissionID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");
            entity.Property(e => e.SubtitleId).HasColumnName("SubtitleID");
            entity.Property(e => e.SubtitleName).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(500);
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__Contents__2907A87E585570FC");

            entity.Property(e => e.ContentId).HasColumnName("ContentID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CommissionId).HasColumnName("CommissionID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");
            entity.Property(e => e.Likes).HasDefaultValue(0);
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.SubtitleId).HasColumnName("SubtitleID");
            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UploadDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.Contents)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contents__Catego__534D60F1");

            entity.HasOne(d => d.Commission).WithMany(p => p.Contents)
                .HasForeignKey(d => d.CommissionId)
                .HasConstraintName("FK__Contents__Commis__656C112C");

            entity.HasOne(d => d.Creator).WithMany(p => p.Contents)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contents__Creato__5629CD9C");

            entity.HasOne(d => d.Event).WithMany(p => p.Contents)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK__Contents__EventI__6477ECF3");

            entity.HasOne(d => d.Plan).WithMany(p => p.Contents)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK__Contents__PlanID__571DF1D5");

            entity.HasOne(d => d.Subtitle).WithMany(p => p.Contents)
                .HasForeignKey(d => d.SubtitleId)
                .HasConstraintName("FK__Contents__Subtit__5441852A");

            entity.HasOne(d => d.Tag).WithMany(p => p.Contents)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK__Contents__TagID__5535A963");
        });

        modelBuilder.Entity<ContentTag>(entity =>
        {
            entity.HasKey(e => e.ContentTagId).HasName("PK__ContentT__8FE5748599013713");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentTags)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("FK__ContentTa__Conte__54CB950F");

            entity.HasOne(d => d.Tag).WithMany(p => p.ContentTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK__ContentTa__TagId__55BFB948");
        });

        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.CreatorId).HasName("PK__Creators__6C7548111D29706E");

            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.BannerUrl).HasColumnName("BannerURL");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C87015B57FAF");

            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EventName).HasMaxLength(200);
            entity.Property(e => e.EventStyle)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__Category__5EBF139D");

            entity.HasOne(d => d.Creator).WithMany(p => p.Events)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Events__CreatorI__3493CFA7");
        });

        modelBuilder.Entity<EventImage>(entity =>
        {
            entity.HasKey(e => e.EventImageId).HasName("PK__EventIma__9B3A6940E9049B02");

            entity.Property(e => e.EventImageId).HasColumnName("EventImageID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.EvePostLike).HasDefaultValue(0);
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.ImageTitle).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

            entity.HasOne(d => d.Creator).WithMany(p => p.EventImages)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventImag__Creat__3587F3E0");

            entity.HasOne(d => d.Event).WithMany(p => p.EventImages)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EventImag__Event__6383C8BA");
        });

        modelBuilder.Entity<EventsAndImage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EventsAndImages");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EveCreName).HasMaxLength(40);
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.EventImageId).HasColumnName("EventImageID");
            entity.Property(e => e.EventName).HasMaxLength(200);
            entity.Property(e => e.EvtImgId).HasColumnName("EvtImgID");
            entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");
            entity.Property(e => e.ImgCreName).HasMaxLength(40);
            entity.Property(e => e.ImgTitle).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => e.FollowId).HasName("PK__Follows__2CE8108E9A59BE23");

            entity.Property(e => e.FollowId).HasColumnName("FollowID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Creator).WithMany(p => p.Follows)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Follows__Creator__68D28DBC");

            entity.HasOne(d => d.User).WithMany(p => p.Follows)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Follows__UserID__67DE6983");
        });

        modelBuilder.Entity<FriendList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FriendLi__3214EC2717DC78BB");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AcceptorUserId).HasColumnName("AcceptorUserID");
            entity.Property(e => e.RequesterUserId).HasColumnName("RequesterUserID");

            entity.HasOne(d => d.AcceptorUser).WithMany(p => p.FriendListAcceptorUsers)
                .HasForeignKey(d => d.AcceptorUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FriendLis__Accep__60A75C0F");

            entity.HasOne(d => d.RequesterUser).WithMany(p => p.FriendListRequesterUsers)
                .HasForeignKey(d => d.RequesterUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FriendLis__Reque__5FB337D6");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Plans__755C22D7617F78FB");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.PlanName).HasMaxLength(100);

            entity.HasOne(d => d.Creator).WithMany(p => p.Plans)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Plans__CreatorID__5DCAEF64");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B24BDD2252C05");

            entity.Property(e => e.SubscriptionId).HasColumnName("SubscriptionID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Subscript__Categ__5AEE82B9");

            entity.HasOne(d => d.Creator).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__Creat__5BE2A6F2");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__PlanI__367C1819");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<Subtitle>(entity =>
        {
            entity.HasKey(e => e.SubtitleId).HasName("PK__Subtitle__A442F915E502DF79");

            entity.Property(e => e.SubtitleId).HasColumnName("SubtitleID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.SubtitleName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Subtitles)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subtitles__Categ__628FA481");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__657CFA4CBEA41F21");

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC6C448E9F");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatorId).HasColumnName("CreatorID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMail");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(40);

            entity.HasOne(d => d.Category).WithMany(p => p.Users)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Users__CategoryI__52593CB8");

            entity.HasOne(d => d.Creator).WithMany(p => p.Users)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__Users__CreatorID__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
