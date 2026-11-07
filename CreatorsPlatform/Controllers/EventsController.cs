using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

/// <summary>
/// 活動 API 控制器。
/// </summary>
[ApiController, Route("api/events")]
public class EventsController(ImaginkContext database) : ApiController(database)
{
    /// <summary>
    /// 活動資料。
    /// </summary>
    /// <param name="Name">名稱</param>
    /// <param name="Description">描述內容</param>
    /// <param name="StartDate">開始時間</param>
    /// <param name="EndDate">結束時間</param>
    /// <param name="CategoryId">分類識別碼</param>
    /// <param name="Banner">活動橫幅</param>
    /// <param name="Samples">範例圖片</param>
    /// <param name="TitleColor">標題顏色</param>
    /// <param name="IntroColor">介紹區顏色</param>
    /// <param name="KeepSamples">保留的範例圖片識別碼</param>
    public record EventData(
        [Required, StringLength(200)] string Name,
        [Required] string Description,
        DateTime StartDate,
        DateTime EndDate,
        int CategoryId,
        string? Banner,
        string[] Samples,
        [RegularExpression("^#[0-9a-fA-F]{6}$")] string TitleColor = "#f5d547",
        [RegularExpression("^#[0-9a-fA-F]{6}$")] string IntroColor = "#ffffff",
        int[]? KeepSamples = null
    );

    /// <summary>
    /// 活動投稿資料。
    /// </summary>
    /// <param name="Title">標題</param>
    /// <param name="Description">描述內容</param>
    /// <param name="Image">圖片資料</param>
    public record EntryData(
        [Required, StringLength(100)] string Title,
        [Required] string Description,
        [Required] string Image
    );

    /// <summary>
    /// 取得活動列表
    /// </summary>
    /// <returns>處理結果</returns>
    [HttpGet]
    public async Task<IActionResult> List() =>
        Ok(
            await Database
                .Events.Where(e => e.EventCancel != true)
                .OrderByDescending(e => e.EventId)
                .Select(e => new
                {
                    id = e.EventId,
                    name = e.EventName,
                    e.StartDate,
                    e.EndDate,
                    e.CreatorId,
                    banner = "/images/event/" + e.EventId,
                    creatorName = e
                        .Creator.Users.OrderBy(u => u.UserId)
                        .Select(u => u.UserName)
                        .FirstOrDefault(),
                })
                .ToListAsync()
        );

    /// <summary>
    /// 取得活動與投稿詳細資料
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <param name="sort">排序方式</param>
    /// <returns>處理結果</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id, string sort = "new")
    {
        var item = await Database
            .Events.Where(e => e.EventId == id && e.EventCancel != true)
            .Select(e => new
            {
                id = e.EventId,
                name = e.EventName,
                e.Description,
                e.StartDate,
                e.EndDate,
                e.CreatorId,
                e.CategoryId,
                e.EventStyle,
                banner = "/images/event/" + e.EventId,
                creatorName = e
                    .Creator.Users.OrderBy(u => u.UserId)
                    .Select(u => u.UserName)
                    .FirstOrDefault(),
            })
            .FirstOrDefaultAsync();
        if (item == null)
            return NotFound();
        var query = Database.EventImages.Where(i => i.EventId == id && i.EveImgCancel != true);
        query =
            sort == "popular"
                ? query.OrderByDescending(i => i.EvePostLike).ThenByDescending(i => i.EventImageId)
            : sort == "old" ? query.OrderBy(i => i.EventImageId)
            : query.OrderByDescending(i => i.EventImageId);
        var images = await query
            .Select(i => new
            {
                id = i.EventImageId,
                title = i.ImageTitle,
                i.Description,
                sample = i.ImageSample,
                image = "/images/event-image/" + i.EventImageId,
                likes = i.EvePostLike,
                i.CreatorId,
                creatorName = i
                    .Creator.Users.OrderBy(u => u.UserId)
                    .Select(u => u.UserName)
                    .FirstOrDefault(),
            })
            .ToListAsync();
        return Ok(new { item, images });
    }

    /// <summary>
    /// 建立或更新活動
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpPost, HttpPut("{id:int}")]
    public async Task<IActionResult> Save(EventData input, int? id = null)
    {
        if (UserId == null)
            return Unauthorized();
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        if (
            input.EndDate <= input.StartDate
            || input.Samples.Length > 10
            || !await Database.Categories.AnyAsync(c => c.CategoryId == input.CategoryId)
        )
            return BadRequest(new { message = "請檢查活動日期與分類。" });
        var item =
            id == null
                ? new Event { CreatorId = creator.Value, EventCancel = false }
                : await Database
                    .Events.Include(e => e.EventImages)
                    .FirstOrDefaultAsync(e =>
                        e.EventId == id && e.CreatorId == creator && e.EventCancel != true
                    );
        if (item == null)
            return NotFound();
        item.EventName = input.Name;
        item.Description = input.Description;
        item.DescriptionString = input.Description;
        item.StartDate = input.StartDate;
        item.EndDate = input.EndDate;
        item.CategoryId = input.CategoryId;
        item.EventStyle = JsonSerializer.Serialize(new[] { input.TitleColor, input.IntroColor });
        if (input.Banner != null)
            item.Banner = input.Banner;
        if (id == null)
            Database.Events.Add(item);
        var keep = input.KeepSamples ?? [];
        if (
            keep.Length + input.Samples.Length > 10
            || keep.Any(sampleId =>
                !item.EventImages.Any(i =>
                    i.EventImageId == sampleId && i.ImageSample && i.EveImgCancel != true
                )
            )
        )
            return BadRequest();
        foreach (
            var sample in item.EventImages.Where(i =>
                i.ImageSample && !keep.Contains(i.EventImageId)
            )
        )
            sample.EveImgCancel = true;
        foreach (var image in input.Samples)
            item.EventImages.Add(
                new EventImage
                {
                    CreatorId = creator.Value,
                    ImageUrl = image,
                    ImageSample = true,
                    EveImgCancel = false,
                }
            );
        await Database.SaveChangesAsync();
        return Ok(new { id = item.EventId });
    }

    /// <summary>
    /// 投稿作品至活動
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("{id:int}/entries")]
    public async Task<IActionResult> Submit(int id, EntryData input)
    {
        if (UserId == null)
            return Unauthorized();
        var creator = await CreatorId();
        if (creator == null)
            return StatusCode(403);
        var now = DateTime.Now;
        if (
            !await Database.Events.AnyAsync(e =>
                e.EventId == id && e.EventCancel != true && e.StartDate <= now && e.EndDate >= now
            )
        )
            return BadRequest(new { message = "目前不在活動投稿期間。" });
        Database.EventImages.Add(
            new EventImage
            {
                EventId = id,
                CreatorId = creator.Value,
                ImageTitle = input.Title,
                Description = input.Description,
                ImageUrl = input.Image,
                ImageSample = false,
                EveImgCancel = false,
                EvePostLike = 0,
            }
        );
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 增加活動投稿的喜歡次數
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpPost("entries/{id:int}/like")]
    public async Task<IActionResult> Like(int id)
    {
        if (UserId == null)
            return Unauthorized();
        var count = await Database
            .EventImages.Where(i =>
                i.EventImageId == id && i.EveImgCancel != true && i.Event.EventCancel != true
            )
            .ExecuteUpdateAsync(s =>
                s.SetProperty(i => i.EvePostLike, i => (i.EvePostLike ?? 0) + 1)
            );
        return count == 0 ? NotFound() : NoContent();
    }
}
