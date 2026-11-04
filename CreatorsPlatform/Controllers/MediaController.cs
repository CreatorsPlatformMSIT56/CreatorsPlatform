using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

public class MediaController(ImaginkContext context) : Controller
{
    [HttpGet("/images/home/{id:int}")]
    public async Task<IActionResult> HomeImage(int id)
    {
        var creatorId = await context.Contents.GroupBy(c => c.CreatorId)
            .OrderByDescending(g => g.Sum(c => c.Likes))
            .Select(g => (int?)g.Key).FirstOrDefaultAsync();
        var image = await context.Contents.AsNoTracking()
            .Where(c => c.CreatorId == creatorId).OrderByDescending(c => c.UploadDate)
            .Take(3).Where(c => c.ContentId == id).Select(c => c.ImageUrl).FirstOrDefaultAsync();
        return ImageResult(image);
    }

    [HttpGet("/images/avatar/{id:int}")]
    public async Task<IActionResult> Avatar(int id)
    {
        var image = await context.Users.AsNoTracking().Where(u => u.UserId == id)
            .Select(u => u.Avatar).FirstOrDefaultAsync();
        return ImageResult(image);
    }

    [HttpGet("/images/content/{id:int}")]
    public async Task<IActionResult> ContentImage(int id)
    {
        var image = await context.Contents.AsNoTracking().Where(c => c.ContentId == id)
            .Select(c => c.ImageUrl).FirstOrDefaultAsync();
        return ImageResult(image);
    }

    [HttpGet("/images/creator-avatar/{id:int}")]
    public async Task<IActionResult> CreatorAvatar(int id)
    {
        var image = await context.Users.AsNoTracking().Where(u => u.CreatorId == id)
            .OrderBy(u => u.UserId).Select(u => u.Avatar).FirstOrDefaultAsync();
        return ImageResult(image);
    }

    [HttpGet("/images/event/{id:int}")]
    public async Task<IActionResult> EventBanner(int id)
    {
        var data = await context.Events.AsNoTracking().Where(e => e.EventId == id)
            .Select(e => e.Banner).FirstOrDefaultAsync();
        return DataImageResult(data);
    }

    [HttpGet("/images/event-image/{id:int}")]
    public async Task<IActionResult> EventImage(int id)
    {
        var data = await context.EventImages.AsNoTracking().Where(e => e.EventImageId == id)
            .Select(e => e.ImageUrl).FirstOrDefaultAsync();
        return DataImageResult(data);
    }

    private IActionResult DataImageResult(string? data)
    {
        if (data == null || !data.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)) return NotFound();
        var comma = data.IndexOf(',');
        if (comma < 0 || !data.AsSpan(0, comma).EndsWith(";base64", StringComparison.OrdinalIgnoreCase)) return NotFound();
        try { return ImageResult(Convert.FromBase64String(data[(comma + 1)..])); }
        catch (FormatException) { return NotFound(); }
    }

    private IActionResult ImageResult(byte[]? image)
    {
        if (image == null) return NotFound();
        var type = image.AsSpan().StartsWith(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a }) ? "image/png"
            : image.AsSpan().StartsWith(new byte[] { 0xff, 0xd8, 0xff }) ? "image/jpeg"
            : image.AsSpan().StartsWith("GIF8"u8) ? "image/gif"
            : image.Length >= 12 && image.AsSpan(0, 4).SequenceEqual("RIFF"u8) && image.AsSpan(8, 4).SequenceEqual("WEBP"u8) ? "image/webp"
            : "application/octet-stream";
        return File(image, type);
    }
}
