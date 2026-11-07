using System.Net;
using CreatorsPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using SkiaSharp;
using Xunit;

namespace CreatorsPlatform.Tests;

/// <summary>
/// 作品縮圖的 HTTP 整合測試。
/// </summary>
public class MediaTests
{
    /// <summary>
    /// 驗證縮圖壓縮、比例及尺寸上限，並保留原圖端點。
    /// </summary>
    [Theory]
    [InlineData(1600, 1200, 800, 600)]
    [InlineData(1200, 1600, 600, 800)]
    [InlineData(120, 80, 120, 80)]
    public async Task ThumbnailIsBoundedWebpAndOriginalIsUnchanged(
        int width, int height, int expectedWidth, int expectedHeight
    )
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        var source = CreateImage(width, height);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ImaginkContext>();
            await db.Contents.Where(c => c.ContentId == 1)
                .ExecuteUpdateAsync(set => set.SetProperty(c => c.ImageUrl, source));
        }
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/images/content/1/thumbnail");
        response.EnsureSuccessStatusCode();
        Assert.Equal("image/webp", response.Content.Headers.ContentType?.MediaType);
        var thumbnail = await response.Content.ReadAsByteArrayAsync();
        using var image = SKBitmap.Decode(thumbnail);
        Assert.Equal(expectedWidth, image.Width);
        Assert.Equal(expectedHeight, image.Height);
        Assert.True(thumbnail.Length < source.Length / 4);
        Assert.Equal(thumbnail, await client.GetByteArrayAsync("/images/content/1/thumbnail"));
        Assert.Equal(source, await client.GetByteArrayAsync("/images/content/1"));
    }

    /// <summary>
    /// 驗證快取中的縮圖也會遵守作品下架狀態。
    /// </summary>
    [Fact]
    public async Task CachedThumbnailIsNotServedAfterWorkIsRemoved()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ImaginkContext>();
        var source = CreateImage(120, 80);
        await db.Contents.Where(c => c.ContentId == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.ImageUrl, source));
        using var client = factory.CreateClient();
        (await client.GetAsync("/images/content/1/thumbnail")).EnsureSuccessStatusCode();
        await db.Contents.Where(c => c.ContentId == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.PullOffDate, DateTime.Now.AddMinutes(-1)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/images/content/1/thumbnail")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/images/content/404/thumbnail")).StatusCode);
    }

    /// <summary>
    /// 驗證缺圖和無效圖檔不會造成伺服器錯誤。
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(new byte[] { 1, 2, 3 })]
    [InlineData(new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a })]
    public async Task MissingOrInvalidImageReturnsNotFound(byte[]? source)
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ImaginkContext>();
        await db.Contents.Where(c => c.ContentId == 1)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.ImageUrl, source));
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/images/content/1/thumbnail")).StatusCode);
    }

    /// <summary>
    /// 建立有固定隨機像素的 PNG，以驗證真實影像解碼與壓縮。
    /// </summary>
    private static byte[] CreateImage(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height, SKColorType.Rgba8888, SKAlphaType.Opaque);
        var pixels = new byte[bitmap.ByteCount];
        new Random(42).NextBytes(pixels);
        Marshal.Copy(pixels, 0, bitmap.GetPixels(), pixels.Length);
        using var encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }
}
