using System.ComponentModel.DataAnnotations;
using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

[ApiController, Route("api/studio")]
public class StudioController(ImaginkContext database) : ApiController(database)
{
    public record CreatorData([Required] string Description, [Required] string Notice);
    public record PostData([Required, StringLength(200)] string Title, [Required] string Description, int CategoryId, int SubtitleId, int PlanId, [Required] string Image);
    public record CommissionData([Required, StringLength(500)] string Title, [Required] string Description, [Range(0, int.MaxValue)] int PriceMin,
        int? PriceMax, int SubtitleId, DateOnly? OverDate, string[] Images);
    public record PlanData([Required, StringLength(100)] string Name, [Range(0, int.MaxValue)] int Price, [Range(0, 100)] int Level, [Required, StringLength(300)] string Description);
    public record CommentData([Required, StringLength(500)] string Text);
    public record OrderData(int CommissionId, [Range(0, int.MaxValue)] int Price, [Required] string Description);
    public record OrderReply([Required] string Action, int? Price);

    [HttpPut("creator")]
    public async Task<IActionResult> SaveCreator(CreatorData input)
    {
        if (UserId == null) return Unauthorized();
        var user = await Database.Users.Include(u => u.Creator).FirstOrDefaultAsync(u => u.UserId == UserId);
        if (user == null) return Unauthorized();
        user.Creator ??= new Creator();
        user.Creator.Description = input.Description; user.Creator.Notice = input.Notice;
        await Database.SaveChangesAsync();
        return Ok(new { id = user.Creator.CreatorId });
    }

    [HttpPost("works")]
    public async Task<IActionResult> CreatePost(PostData input)
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        if (creator == null) return StatusCode(403);
        if (!await Database.Plans.AnyAsync(p => p.PlanId == input.PlanId && p.CreatorId == creator) ||
            !await Database.Subtitles.AnyAsync(s => s.SubtitleId == input.SubtitleId && s.CategoryId == input.CategoryId)) return BadRequest();
        byte[] image;
        try { image = Convert.FromBase64String(input.Image); } catch (FormatException) { return BadRequest(); }
        if (image.Length > 5 * 1024 * 1024) return BadRequest();
        var content = new Content { Title = input.Title, Description = input.Description, CreatorId = creator.Value,
            CategoryId = input.CategoryId, SubtitleId = input.SubtitleId, PlanId = input.PlanId, ImageUrl = image, UploadDate = DateTime.Now, Likes = 0 };
        Database.Contents.Add(content);
        await Database.SaveChangesAsync();
        return Ok(new { id = content.ContentId });
    }

    [HttpPost("commissions")]
    public async Task<IActionResult> CreateCommission(CommissionData input)
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        if (creator == null) return StatusCode(403);
        if (input.PriceMax < input.PriceMin || !await Database.Subtitles.AnyAsync(s => s.SubtitleId == input.SubtitleId) || input.Images.Length > 10) return BadRequest();
        var item = new Commission { Title = input.Title, Description = input.Description, PriceMin = input.PriceMin,
            PriceMax = input.PriceMax, SubtitleId = input.SubtitleId, OverDate = input.OverDate,
            CreatorId = creator.Value, PutUpDate = DateOnly.FromDateTime(DateTime.Today),
            CommissionImages = input.Images.Select(i => new CommissionImage { ImageUrl = i }).ToList() };
        Database.Commissions.Add(item);
        await Database.SaveChangesAsync();
        return Ok(new { id = item.CommissionId });
    }

    [HttpPut("plans")]
    public async Task<IActionResult> SavePlan(PlanData input)
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        if (creator == null) return StatusCode(403);
        var plan = await Database.Plans.FirstOrDefaultAsync(p => p.CreatorId == creator && p.PlanLevel == input.Level);
        if (plan == null) { plan = new Plan { CreatorId = creator.Value, PlanLevel = input.Level }; Database.Plans.Add(plan); }
        plan.PlanName = input.Name; plan.PlanPrice = input.Price; plan.Description = input.Description;
        await Database.SaveChangesAsync();
        return Ok(new { id = plan.PlanId });
    }

    [HttpPost("creators/{id:int}/follow")]
    public async Task<IActionResult> Follow(int id)
    {
        if (UserId == null) return Unauthorized();
        if (!await Database.Creators.AnyAsync(c => c.CreatorId == id)) return NotFound();
        var follow = await Database.Follows.FirstOrDefaultAsync(f => f.UserId == UserId && f.CreatorId == id);
        if (follow == null) { follow = new Follow { UserId = UserId.Value, CreatorId = id, Unfollow = false }; Database.Follows.Add(follow); }
        else follow.Unfollow = follow.Unfollow != true;
        await Database.SaveChangesAsync();
        return Ok(new { following = follow.Unfollow != true });
    }

    [HttpPost("works/{id:int}/comments")]
    public async Task<IActionResult> Comment(int id, CommentData input)
    {
        if (UserId == null) return Unauthorized();
        if (!await Database.Contents.AnyAsync(c => c.ContentId == id)) return NotFound();
        Database.Comments.Add(new Comment { ContentId = id, UserId = UserId.Value, Comment1 = input.Text });
        await Database.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("works/{id:int}/like")]
    public async Task<IActionResult> Like(int id)
    {
        if (UserId == null) return Unauthorized();
        var count = await Database.Contents.Where(c => c.ContentId == id).ExecuteUpdateAsync(s => s.SetProperty(c => c.Likes, c => (c.Likes ?? 0) + 1));
        return count == 0 ? NotFound() : NoContent();
    }

    [HttpPost("plans/{id:int}/subscribe")]
    public async Task<IActionResult> Subscribe(int id)
    {
        if (UserId == null) return Unauthorized();
        var plan = await Database.Plans.FindAsync(id);
        if (plan == null) return NotFound();
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (await Database.Subscriptions.AnyAsync(s => s.UserId == UserId && s.PlanId == id && s.EndDate >= today))
            return Conflict(new { message = "您已訂閱此方案。" });
        Database.Subscriptions.Add(new Subscription { UserId = UserId.Value, CreatorId = plan.CreatorId, PlanId = id,
            StartDate = today, EndDate = today.AddMonths(1), PaymentMade = true });
        var follow = await Database.Follows.FirstOrDefaultAsync(f => f.UserId == UserId && f.CreatorId == plan.CreatorId);
        if (follow == null) Database.Follows.Add(new Follow { UserId = UserId.Value, CreatorId = plan.CreatorId, Unfollow = false });
        else follow.Unfollow = false;
        await Database.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("orders")]
    public async Task<IActionResult> Order(OrderData input)
    {
        if (UserId == null) return Unauthorized();
        var commission = await Database.Commissions.FindAsync(input.CommissionId);
        if (commission == null) return NotFound();
        if (commission.OverDate < DateOnly.FromDateTime(DateTime.Today)) return BadRequest(new { message = "委託已截止。" });
        Database.CommissionOrders.Add(new CommissionOrder { UserId = UserId.Value, CommissionId = input.CommissionId,
            Price = input.Price, Description = input.Description, OrderDate = DateOnly.FromDateTime(DateTime.Today), WorkStatus = "待確認中" });
        await Database.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("orders/{id:int}")]
    public async Task<IActionResult> Reply(int id, OrderReply input)
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        var order = await Database.CommissionOrders.Include(o => o.Commission)
            .FirstOrDefaultAsync(o => o.CommissionOrderId == id && (o.UserId == UserId || o.Commission.CreatorId == creator));
        if (order == null) return NotFound();
        var owner = order.Commission.CreatorId == creator;
        var next = (owner, order.WorkStatus, input.Action) switch
        {
            (true, "待確認中", "accept") when input.Price >= 0 => "價格回覆",
            (false, "價格回覆", "accept") => "價格確認",
            (true, "價格確認", "accept") => "製作中",
            (true, "製作中", "accept") => "完成",
            (true, "待確認中" or "價格確認" or "製作中", "reject") => "拒絕",
            (false, "價格回覆", "reject") => "拒絕",
            _ => null
        };
        if (next == null) return BadRequest();
        if (next == "價格回覆") order.Price = input.Price!.Value;
        order.WorkStatus = next;
        await Database.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{kind}/{id:int}")]
    public async Task<IActionResult> Delete(string kind, int id)
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        if (creator == null) return StatusCode(403);
        if (kind == "works")
        {
            var count = await Database.Contents.Where(c => c.ContentId == id && c.CreatorId == creator)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.PullOffDate, DateTime.Now));
            return count == 0 ? NotFound() : NoContent();
        }
        if (kind == "events")
        {
            var count = await Database.Events.Where(e => e.EventId == id && e.CreatorId == creator)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.EventCancel, true));
            return count == 0 ? NotFound() : NoContent();
        }
        if (kind == "plans")
        {
            var plan = await Database.Plans.FirstOrDefaultAsync(p => p.PlanId == id && p.CreatorId == creator);
            if (plan == null) return NotFound();
            if (await Database.Contents.AnyAsync(c => c.PlanId == id) || await Database.Subscriptions.AnyAsync(s => s.PlanId == id))
                return Conflict(new { message = "方案已有作品或訂閱，無法刪除。" });
            Database.Plans.Remove(plan); await Database.SaveChangesAsync(); return NoContent();
        }
        return BadRequest();
    }
}
