using System.ComponentModel.DataAnnotations;
using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

/// <summary>
/// 創作及互動 API 控制器。
/// </summary>
[Authorize]
[ApiController, Route("api/studio")]
public class StudioController(ImaginkContext database) : ApiController(database)
{
    /// <summary>
    /// 創作者資料。
    /// </summary>
    /// <param name="Description">描述內容</param>
    /// <param name="Notice">公告內容</param>
    public record CreatorData([Required] string Description, [Required] string Notice);

    /// <summary>
    /// 作品資料。
    /// </summary>
    /// <param name="Title">標題</param>
    /// <param name="Description">描述內容</param>
    /// <param name="CategoryId">分類識別碼</param>
    /// <param name="SubtitleId">子分類識別碼</param>
    /// <param name="PlanId">方案識別碼</param>
    /// <param name="Image">圖片資料</param>
    public record PostData(
        [Required, StringLength(200)] string Title,
        [Required] string Description,
        int CategoryId,
        int SubtitleId,
        int PlanId,
        [Required] string Image
    );

    /// <summary>
    /// 委託項目資料。
    /// </summary>
    /// <param name="Title">標題</param>
    /// <param name="Description">描述內容</param>
    /// <param name="PriceMin">最低價格</param>
    /// <param name="PriceMax">最高價格</param>
    /// <param name="SubtitleId">子分類識別碼</param>
    /// <param name="OverDate">截止日期</param>
    /// <param name="Images">圖片資料集合</param>
    public record CommissionData(
        [Required, StringLength(500)] string Title,
        [Required] string Description,
        [Range(0, int.MaxValue)] int PriceMin,
        int? PriceMax,
        int SubtitleId,
        DateOnly? OverDate,
        string[] Images
    );

    /// <summary>
    /// 訂閱方案資料。
    /// </summary>
    /// <param name="Name">名稱</param>
    /// <param name="Price">價格</param>
    /// <param name="Level">等級</param>
    /// <param name="Description">描述內容</param>
    public record PlanData(
        [Required, StringLength(100)] string Name,
        [Range(0, int.MaxValue)] int Price,
        [Range(0, 100)] int Level,
        [Required, StringLength(300)] string Description
    );

    /// <summary>
    /// 留言資料。
    /// </summary>
    /// <param name="Text">文字內容</param>
    public record CommentData([Required, StringLength(500)] string Text);

    /// <summary>
    /// 委託訂單資料。
    /// </summary>
    /// <param name="CommissionId">委託識別碼</param>
    /// <param name="Price">價格</param>
    /// <param name="Description">描述內容</param>
    public record OrderData(
        int CommissionId,
        [Range(0, int.MaxValue)] int Price,
        [Required] string Description
    );

    /// <summary>
    /// 委託訂單回覆資料。
    /// </summary>
    /// <param name="Action">回覆動作</param>
    /// <param name="Price">價格</param>
    public record OrderReply([Required] string Action, int? Price);

    /// <summary>
    /// 儲存創作者資料
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPut("creator")]
    public async Task<IActionResult> SaveCreator(CreatorData input)
    {
        var user = await Database
            .Users.Include(u => u.Creator)
            .FirstOrDefaultAsync(u => u.UserId == UserId);
        if (user == null)
            return Unauthorized();
        user.Creator ??= new Creator();
        user.Creator.Description = input.Description;
        user.Creator.Notice = input.Notice;
        await Database.SaveChangesAsync();
        return Ok(new { id = user.Creator.CreatorId });
    }

    /// <summary>
    /// 建立作品
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("works")]
    public async Task<IActionResult> CreatePost(PostData input)
    {
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        if (
            !await Database.Plans.AnyAsync(p => p.PlanId == input.PlanId && p.CreatorId == creator)
            || !await Database.Subtitles.AnyAsync(s =>
                s.SubtitleId == input.SubtitleId && s.CategoryId == input.CategoryId
            )
        )
            return BadRequest();
        byte[] image;
        try
        {
            image = Convert.FromBase64String(input.Image);
        }
        catch (FormatException)
        {
            return BadRequest();
        }
        if (image.Length > 5 * 1024 * 1024)
            return BadRequest();
        var content = new Content
        {
            Title = input.Title,
            Description = input.Description,
            CreatorId = creator.Value,
            CategoryId = input.CategoryId,
            SubtitleId = input.SubtitleId,
            PlanId = input.PlanId,
            ImageUrl = image,
            UploadDate = DateTime.Now,
            Likes = 0,
        };
        Database.Contents.Add(content);
        await Database.SaveChangesAsync();
        return Ok(new { id = content.ContentId });
    }

    /// <summary>
    /// 建立委託項目
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("commissions")]
    public async Task<IActionResult> CreateCommission(CommissionData input)
    {
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        if (
            input.PriceMax < input.PriceMin
            || !await Database.Subtitles.AnyAsync(s => s.SubtitleId == input.SubtitleId)
            || input.Images.Length > 10
        )
            return BadRequest();
        var item = new Commission
        {
            Title = input.Title,
            Description = input.Description,
            PriceMin = input.PriceMin,
            PriceMax = input.PriceMax,
            SubtitleId = input.SubtitleId,
            OverDate = input.OverDate,
            CreatorId = creator.Value,
            PutUpDate = DateOnly.FromDateTime(DateTime.Today),
            CommissionImages = input
                .Images.Select(i => new CommissionImage { ImageUrl = i })
                .ToList(),
        };
        Database.Commissions.Add(item);
        await Database.SaveChangesAsync();
        return Ok(new { id = item.CommissionId });
    }

    /// <summary>
    /// 儲存訂閱方案
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPut("plans")]
    public async Task<IActionResult> SavePlan(PlanData input)
    {
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        var plan = await Database.Plans.FirstOrDefaultAsync(p =>
            p.CreatorId == creator && p.PlanLevel == input.Level
        );
        if (plan == null)
        {
            plan = new Plan { CreatorId = creator.Value, PlanLevel = input.Level };
            Database.Plans.Add(plan);
        }
        plan.PlanName = input.Name;
        plan.PlanPrice = input.Price;
        plan.Description = input.Description;
        await Database.SaveChangesAsync();
        return Ok(new { id = plan.PlanId });
    }

    /// <summary>
    /// 切換對創作者的追蹤狀態
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpPost("creators/{id:int}/follow")]
    public async Task<IActionResult> Follow(int id)
    {
        if (!await Database.Creators.AnyAsync(c => c.CreatorId == id))
            return NotFound();
        var follow = await Database.Follows.FirstOrDefaultAsync(f =>
            f.UserId == UserId && f.CreatorId == id
        );
        if (follow == null)
        {
            follow = new Follow
            {
                UserId = UserId!.Value,
                CreatorId = id,
                Unfollow = false,
            };
            Database.Follows.Add(follow);
        }
        else
            follow.Unfollow = follow.Unfollow != true;
        await Database.SaveChangesAsync();
        return Ok(new { following = follow.Unfollow != true });
    }

    /// <summary>
    /// 新增作品留言
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("works/{id:int}/comments")]
    public async Task<IActionResult> Comment(int id, CommentData input)
    {
        if (!await Database.Contents.AnyAsync(c => c.ContentId == id))
            return NotFound();
        Database.Comments.Add(
            new Comment
            {
                ContentId = id,
                UserId = UserId!.Value,
                Comment1 = input.Text,
            }
        );
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 增加作品的喜歡次數
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpPost("works/{id:int}/like")]
    public async Task<IActionResult> Like(int id)
    {
        var count = await Database
            .Contents.Where(c => c.ContentId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Likes, c => (c.Likes ?? 0) + 1));
        return count == 0 ? NotFound() : NoContent();
    }

    /// <summary>
    /// 訂閱方案
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpPost("plans/{id:int}/subscribe")]
    public async Task<IActionResult> Subscribe(int id)
    {
        var plan = await Database.Plans.FindAsync(id);
        if (plan == null)
            return NotFound();
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (
            await Database.Subscriptions.AnyAsync(s =>
                s.UserId == UserId && s.PlanId == id && s.EndDate >= today
            )
        )
            return Conflict(new { message = "您已訂閱此方案。" });
        Database.Subscriptions.Add(
            new Subscription
            {
                UserId = UserId!.Value,
                CreatorId = plan.CreatorId,
                PlanId = id,
                StartDate = today,
                EndDate = today.AddMonths(1),
                PaymentMade = true,
            }
        );
        var follow = await Database.Follows.FirstOrDefaultAsync(f =>
            f.UserId == UserId && f.CreatorId == plan.CreatorId
        );
        if (follow == null)
            Database.Follows.Add(
                new Follow
                {
                    UserId = UserId!.Value,
                    CreatorId = plan.CreatorId,
                    Unfollow = false,
                }
            );
        else
            follow.Unfollow = false;
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 建立委託訂單
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("orders")]
    public async Task<IActionResult> Order(OrderData input)
    {
        var commission = await Database.Commissions.FindAsync(input.CommissionId);
        if (commission == null)
            return NotFound();
        if (commission.OverDate < DateOnly.FromDateTime(DateTime.Today))
            return BadRequest(new { message = "委託已截止。" });
        Database.CommissionOrders.Add(
            new CommissionOrder
            {
                UserId = UserId!.Value,
                CommissionId = input.CommissionId,
                Price = input.Price,
                Description = input.Description,
                OrderDate = DateOnly.FromDateTime(DateTime.Today),
                WorkStatus = "待確認中",
            }
        );
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 回覆委託訂單
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPut("orders/{id:int}")]
    public async Task<IActionResult> Reply(int id, OrderReply input)
    {
        var creator = await CreatorId();
        var order = await Database
            .CommissionOrders.Include(o => o.Commission)
            .FirstOrDefaultAsync(o =>
                o.CommissionOrderId == id
                && (o.UserId == UserId || o.Commission.CreatorId == creator)
            );
        if (order == null)
            return NotFound();
        var owner = order.Commission.CreatorId == creator;
        var next = (owner, order.WorkStatus, input.Action) switch
        {
            (true, "待確認中", "accept") when input.Price >= 0 => "價格回覆",
            (false, "價格回覆", "accept") => "價格確認",
            (true, "價格確認", "accept") => "製作中",
            (true, "製作中", "accept") => "完成",
            (true, "待確認中" or "價格確認" or "製作中", "reject") => "拒絕",
            (false, "價格回覆", "reject") => "拒絕",
            _ => null,
        };
        if (next == null)
            return BadRequest();
        if (next == "價格回覆")
            order.Price = input.Price!.Value;
        order.WorkStatus = next;
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 刪除指定的管理項目
    /// </summary>
    /// <param name="kind">管理項目的類型</param>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpDelete("{kind}/{id:int}")]
    public async Task<IActionResult> Delete(string kind, int id)
    {
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        if (kind == "works")
        {
            var count = await Database
                .Contents.Where(c => c.ContentId == id && c.CreatorId == creator)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.PullOffDate, DateTime.Now));
            return count == 0 ? NotFound() : NoContent();
        }
        if (kind == "events")
        {
            var count = await Database
                .Events.Where(e => e.EventId == id && e.CreatorId == creator)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.EventCancel, true));
            return count == 0 ? NotFound() : NoContent();
        }
        if (kind == "plans")
        {
            var plan = await Database.Plans.FirstOrDefaultAsync(p =>
                p.PlanId == id && p.CreatorId == creator
            );
            if (plan == null)
                return NotFound();
            if (
                await Database.Contents.AnyAsync(c => c.PlanId == id)
                || await Database.Subscriptions.AnyAsync(s => s.PlanId == id)
            )
                return Conflict(new { message = "方案已有作品或訂閱，無法刪除。" });
            Database.Plans.Remove(plan);
            await Database.SaveChangesAsync();
            return NoContent();
        }
        return BadRequest();
    }
}
