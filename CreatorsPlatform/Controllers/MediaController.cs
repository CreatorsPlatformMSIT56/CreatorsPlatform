using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;

namespace CreatorsPlatform.Controllers;

/// <summary>
/// 圖片資源控制器。
/// </summary>
public class MediaController(ImaginkContext context, IMemoryCache cache) : Controller
{
    /// <summary>
    /// 取得使用者大頭貼
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpGet("/images/avatar/{id:int}")]
    public async Task<IActionResult> Avatar(int id)
    {
        var image = await context
            .Users.AsNoTracking()
            .Where(u => u.UserId == id)
            .Select(u => u.Avatar)
            .FirstOrDefaultAsync();
        return ImageResult(image);
    }

    /// <summary>
    /// 取得作品圖片
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpGet("/images/content/{id:int}")]
    public async Task<IActionResult> ContentImage(int id)
    {
        var image = await context
            .Contents.AsNoTracking()
            .Where(c =>
                c.ContentId == id && (c.PullOffDate == null || c.PullOffDate > DateTime.Now)
            )
            .Select(c => c.ImageUrl)
            .FirstOrDefaultAsync();
        return ImageResult(image);
    }

    /// <summary>
    /// 取得作品卡片使用的壓縮縮圖，最長邊不超過 800 像素。
    /// </summary>
    /// <param name="id">作品識別碼</param>
    /// <returns>WebP 縮圖；作品已下架或圖片無效時回傳 404</returns>
    [HttpGet("/images/content/{id:int}/thumbnail")]
    public async Task<IActionResult> ContentThumbnail(int id)
    {
        var content = context.Contents.AsNoTracking().Where(c =>
            c.ContentId == id && (c.PullOffDate == null || c.PullOffDate > DateTime.Now)
        );
        // Check publication on every request, including cache hits.
        if (!await content.AnyAsync(c => c.ImageUrl != null))
            return NotFound();

        var thumbnail = await cache.GetOrCreateAsync<byte[]?>(
            ("content-thumbnail", id),
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                var source = await content.Select(c => c.ImageUrl).FirstOrDefaultAsync();
                if (source == null)
                    return null;
                // SKImage decodes the first frame and applies EXIF orientation.
                using var image = SKImage.FromEncodedData(source);
                if (image == null)
                    return null;
                var scale = Math.Min(1, 800d / Math.Max(image.Width, image.Height));
                using var bitmap = new SKBitmap(
                    Math.Max(1, (int)Math.Round(image.Width * scale)),
                    Math.Max(1, (int)Math.Round(image.Height * scale)),
                    SKColorType.Rgba8888,
                    SKAlphaType.Premul
                );
                using var pixels = bitmap.PeekPixels();
                if (!image.ScalePixels(pixels, new SKSamplingOptions(SKFilterMode.Linear)))
                    return null;
                using var encoded = pixels.Encode(
                    new SKWebpEncoderOptions(SKWebpEncoderCompression.Lossy, 75)
                );
                return encoded?.ToArray();
            }
        );
        return thumbnail == null ? NotFound() : File(thumbnail, "image/webp");
    }

    /// <summary>
    /// 取得創作者大頭貼
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpGet("/images/creator-avatar/{id:int}")]
    public async Task<IActionResult> CreatorAvatar(int id)
    {
        var image = await context
            .Users.AsNoTracking()
            .Where(u => u.CreatorId == id)
            .OrderBy(u => u.UserId)
            .Select(u => u.Avatar)
            .FirstOrDefaultAsync();
        return ImageResult(image);
    }

    /// <summary>
    /// 取得活動橫幅
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpGet("/images/event/{id:int}")]
    public async Task<IActionResult> EventBanner(int id)
    {
        var data = await context
            .Events.AsNoTracking()
            .Where(e => e.EventId == id && e.EventCancel != true)
            .Select(e => e.Banner)
            .FirstOrDefaultAsync();
        return DataImageResult(data);
    }

    /// <summary>
    /// 取得活動投稿圖片
    /// </summary>
    /// <param name="id">目標項目的識別碼</param>
    /// <returns>處理結果</returns>
    [HttpGet("/images/event-image/{id:int}")]
    public async Task<IActionResult> EventImage(int id)
    {
        var data = await context
            .EventImages.AsNoTracking()
            .Where(e =>
                e.EventImageId == id && e.EveImgCancel != true && e.Event.EventCancel != true
            )
            .Select(e => e.ImageUrl)
            .FirstOrDefaultAsync();
        return DataImageResult(data);
    }

    /// <summary>
    /// 將圖片資料網址轉換為圖片回應
    /// </summary>
    /// <param name="data">圖片資料網址</param>
    /// <returns>處理結果</returns>
    private IActionResult DataImageResult(string? data)
    {
        if (data == null || !data.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            return NotFound();
        var comma = data.IndexOf(',');
        if (
            comma < 0
            || !data.AsSpan(0, comma).EndsWith(";base64", StringComparison.OrdinalIgnoreCase)
        )
            return NotFound();
        try
        {
            return ImageResult(Convert.FromBase64String(data[(comma + 1)..]));
        }
        catch (FormatException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// 依圖片內容判斷格式並建立回應
    /// </summary>
    /// <param name="image">圖片位元組</param>
    /// <returns>處理結果</returns>
    private IActionResult ImageResult(byte[]? image)
    {
        if (image == null)
            return NotFound();
        var type =
            image.AsSpan().StartsWith(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a })
                ? "image/png"
            : image.AsSpan().StartsWith(new byte[] { 0xff, 0xd8, 0xff }) ? "image/jpeg"
            : image.AsSpan().StartsWith("GIF8"u8) ? "image/gif"
            : image.Length >= 12
            && image.AsSpan(0, 4).SequenceEqual("RIFF"u8)
            && image.AsSpan(8, 4).SequenceEqual("WEBP"u8)
                ? "image/webp"
            : "application/octet-stream";
        return File(image, type);
    }
}
