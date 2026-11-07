using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CreatorsPlatform.Tests;

// Each test creates a fresh relational database; no test can touch appsettings' SQL Server.
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        connection.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ImaginkContext>();
            services.RemoveAll<DbContextOptions<ImaginkContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ImaginkContext>>();
            services.AddScoped<ImaginkContext>(_ => new TestContext(
                new DbContextOptionsBuilder<ImaginkContext>().UseSqlite(connection).Options));
        });
    }

    public async Task Seed()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ImaginkContext>();
        await db.Database.EnsureCreatedAsync();
        var creator = new Creator { CreatorId = 1, Description = "Creator", Notice = "Notice" };
        db.Categories.Add(new Category { CategoryId = 1, CategoryName = "Art" });
        db.Subtitles.Add(new Subtitle { SubtitleId = 1, CategoryId = 1, SubtitleName = "Original" });
        db.Creators.Add(creator);
        db.Creators.Add(new Creator { CreatorId = 2, Description = "Other creator", Notice = "Notice" });
        db.Users.AddRange(new User { UserId = 1, UserName = "Owner", Email = "owner@example.test", Password = "password1", Creator = creator },
            new User { UserId = 2, UserName = "Reader", Email = "reader@example.test", Password = "password2" },
            new User { UserId = 3, UserName = "Other", Email = "other@example.test", Password = "password3", CreatorId = 2 });
        db.Plans.Add(new Plan { PlanId = 1, CreatorId = 1, PlanName = "Support", PlanLevel = 0, PlanPrice = 100, Description = "Plan" });
        db.Contents.Add(new Content { ContentId = 1, CreatorId = 1, CategoryId = 1, SubtitleId = 1, PlanId = 1,
            Title = "First work", Description = "Description", UploadDate = DateTime.Now, ImageUrl = [0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a], Likes = 0 });
        db.Commissions.Add(new Commission { CommissionId = 1, CreatorId = 1, Title = "Commission", Description = "Description", SubtitleId = 1,
            PriceMin = 100, PutUpDate = DateOnly.FromDateTime(DateTime.Today) });
        await db.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) connection.Dispose();
    }

    private sealed class TestContext(DbContextOptions<ImaginkContext> options) : ImaginkContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Content>().Property(c => c.UploadDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
