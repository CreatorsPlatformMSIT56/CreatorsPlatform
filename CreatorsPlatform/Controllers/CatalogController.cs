using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

[ApiController, Route("api/catalog")]
public class CatalogController(ImaginkContext database) : ApiController(database)
{
    [HttpGet("lookups")]
    public async Task<IActionResult> Lookups() => Ok(new
    {
        categories = await Database.Categories.Select(c => new { id = c.CategoryId, name = c.CategoryName }).ToListAsync(),
        subtitles = await Database.Subtitles.Select(s => new { id = s.SubtitleId, name = s.SubtitleName, s.CategoryId }).ToListAsync()
    });

    [HttpGet("creators")]
    public async Task<IActionResult> Creators() => Ok(await Database.Creators.Select(c => new
    {
        id = c.CreatorId, name = c.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
        c.Description, avatar = "/images/creator-avatar/" + c.CreatorId,
        followers = c.Follows.Count(f => f.Unfollow != true)
    }).OrderByDescending(c => c.followers).Take(24).ToListAsync());

    [HttpGet("works")]
    public async Task<IActionResult> Works(string q = "", int? subtitle = null, string sort = "new", int page = 1, int? creator = null, bool following = false, int? tag = null)
    {
        if (page < 1) return BadRequest();
        var query = Database.Contents.AsNoTracking().Where(c => c.PullOffDate == null || c.PullOffDate > DateTime.Now);
        if (following)
        {
            if (UserId == null) return Unauthorized();
            query = query.Where(c => c.Creator.Follows.Any(f => f.UserId == UserId && f.Unfollow != true));
        }
        if (creator != null) query = query.Where(c => c.CreatorId == creator);
        if (subtitle != null) query = query.Where(c => c.SubtitleId == subtitle);
        if (tag != null) query = query.Where(c => c.ContentTags.Any(t => t.TagId == tag));
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(c => c.Title.Contains(q) || c.Creator.Users.Any(u => u.UserName.Contains(q)));
        var total = await query.CountAsync();
        query = sort == "popular" ? query.OrderByDescending(c => c.Likes).ThenByDescending(c => c.ContentId)
            : sort == "old" ? query.OrderBy(c => c.ContentId) : query.OrderByDescending(c => c.ContentId);
        return Ok(new { total, page, pageSize = 12, items = await query.Skip((page - 1) * 12).Take(12).Select(c => new
        {
            id = c.ContentId, c.Title, c.CreatorId, creatorName = c.Creator.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
            image = "/images/content/" + c.ContentId, c.Likes, c.UploadDate, subtitle = c.Subtitle == null ? null : c.Subtitle.SubtitleName,
            planLevel = c.Plan.PlanLevel
        }).ToListAsync() });
    }

    [HttpGet("commissions")]
    public async Task<IActionResult> Commissions(string q = "", int? subtitle = null, string sort = "new", int page = 1, int? creator = null)
    {
        if (page < 1) return BadRequest();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = Database.Commissions.AsNoTracking().Where(c => c.OverDate == null || c.OverDate >= today);
        if (creator != null) query = query.Where(c => c.CreatorId == creator);
        if (subtitle != null) query = query.Where(c => c.SubtitleId == subtitle);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(c => c.Title!.Contains(q) || c.Creator.Users.Any(u => u.UserName.Contains(q)));
        var total = await query.CountAsync();
        query = sort == "price" ? query.OrderBy(c => c.PriceMin).ThenByDescending(c => c.CommissionId) : query.OrderByDescending(c => c.CommissionId);
        return Ok(new { total, page, pageSize = 12, items = await query.Skip((page - 1) * 12).Take(12).Select(c => new
        {
            id = c.CommissionId, c.Title, c.CreatorId, creatorName = c.Creator.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
            image = c.CommissionImages.OrderBy(i => i.CommissionImageId).Select(i => i.ImageUrl).FirstOrDefault(), c.PriceMin, c.PriceMax
        }).ToListAsync() });
    }

    [HttpGet("creators/{id:int}")]
    public async Task<IActionResult> Creator(int id)
    {
        var creator = await Database.Creators.Where(c => c.CreatorId == id).Select(c => new
        {
            id = c.CreatorId, name = c.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
            c.Description, c.Notice, c.BannerUrl, avatar = "/images/creator-avatar/" + c.CreatorId,
            following = c.Follows.Any(f => f.UserId == UserId && f.Unfollow != true),
            followers = c.Follows.Count(f => f.Unfollow != true),
            plans = c.Plans.OrderBy(p => p.PlanLevel).Select(p => new { id = p.PlanId, name = p.PlanName, price = p.PlanPrice, level = p.PlanLevel, p.Description }).ToList()
        }).FirstOrDefaultAsync();
        return creator == null ? NotFound() : Ok(creator);
    }

    [HttpGet("works/{id:int}")]
    public async Task<IActionResult> Work(int id)
    {
        var content = await Database.Contents.AsNoTracking().Where(c => c.ContentId == id && (c.PullOffDate == null || c.PullOffDate > DateTime.Now))
            .Select(c => new { id = c.ContentId, c.Title, c.Description, c.CreatorId, c.Likes, c.UploadDate,
                creatorName = c.Creator.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
                image = "/images/content/" + c.ContentId,
                tags = c.ContentTags.Select(t => new { id = t.TagId, name = t.Tag!.TagName }).ToList(),
                comments = c.Comments.OrderByDescending(m => m.CommentId).Select(m => new { id = m.CommentId, text = m.Comment1, name = m.User.UserName, avatar = "/images/avatar/" + m.UserId }).ToList()
            }).FirstOrDefaultAsync();
        return content == null ? NotFound() : Ok(content);
    }

    [HttpGet("commissions/{id:int}")]
    public async Task<IActionResult> Commission(int id)
    {
        var item = await Database.Commissions.Where(c => c.CommissionId == id).Select(c => new
        {
            id = c.CommissionId, c.Title, c.Description, c.CreatorId, c.PriceMin, c.PriceMax, c.OverDate,
            creatorName = c.Creator.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault(),
            images = c.CommissionImages.OrderBy(i => i.CommissionImageId).Select(i => i.ImageUrl).ToList()
        }).FirstOrDefaultAsync();
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("plans/{id:int}")]
    public async Task<IActionResult> Plan(int id)
    {
        var item = await Database.Plans.Where(p => p.PlanId == id).Select(p => new
        {
            id = p.PlanId, name = p.PlanName, price = p.PlanPrice, p.Description, p.CreatorId,
            creatorName = p.Creator.Users.OrderBy(u => u.UserId).Select(u => u.UserName).FirstOrDefault()
        }).FirstOrDefaultAsync();
        return item == null ? NotFound() : Ok(item);
    }
}
