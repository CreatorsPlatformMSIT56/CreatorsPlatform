using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace CreatorsPlatform.Tests;

/// <summary>
/// API 整合測試。
/// </summary>
public class ApiTests
{
    /// <summary>
    /// 以測試讀者或作品擁有者身分登入。
    /// </summary>
    /// <param name="client">用於保存登入工作階段的 HTTP 用戶端</param>
    /// <param name="owner">是否以作品擁有者身分登入</param>
    /// <returns>表示登入作業的非同步工作</returns>
    private static async Task Login(HttpClient client, bool owner = false)
    {
        var response = await client.PostAsJsonAsync(
            "/api/account/login",
            new
            {
                email = owner ? "owner@example.test" : "reader@example.test",
                password = owner ? "password1" : "password2",
            }
        );
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// 以另一位創作者的身分登入。
    /// </summary>
    /// <param name="client">用於保存登入工作階段的 HTTP 用戶端</param>
    /// <returns>表示登入作業的非同步工作</returns>
    private static async Task LoginOtherCreator(HttpClient client) =>
        (
            await client.PostAsJsonAsync(
                "/api/account/login",
                new { email = "other@example.test", password = "password3" }
            )
        ).EnsureSuccessStatusCode();

    /// <summary>
    /// 傳送附帶 JSON 內容的刪除請求。
    /// </summary>
    /// <param name="client">傳送請求的 HTTP 用戶端</param>
    /// <param name="path">刪除請求的路徑</param>
    /// <returns>包含 HTTP 回應的非同步工作</returns>
    private static Task<HttpResponseMessage> Delete(HttpClient client, string path) =>
        client.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, path)
            {
                Content = JsonContent.Create(new { }),
            }
        );

    /// <summary>
    /// 驗證登入後的使用者資訊不包含密碼，且登出後無法存取管理頁面。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task CookieAuthenticationRoundtripDoesNotExposePassword()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var client = factory.CreateClient();
        await Login(client);
        var response = await client.GetAsync("/api/account");
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("reader@example.test", body);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(2, JsonDocument.Parse(body).RootElement.GetProperty("id").GetInt32());
        (await client.PostAsJsonAsync("/api/account/logout", new { })).EnsureSuccessStatusCode();
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await client.GetAsync("/api/dashboard")).StatusCode
        );
    }

    /// <summary>
    /// 驗證異動請求需要 JSON 格式及已登入的工作階段。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task MutationRequiresJsonAndAuthentication()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var client = factory.CreateClient();
        Assert.Equal(
            HttpStatusCode.UnsupportedMediaType,
            (
                await client.PostAsync("/api/studio/works/1/like", new FormUrlEncodedContent([]))
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            (await client.PostAsJsonAsync("/api/studio/works/1/like", new { })).StatusCode
        );
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/yhu/IMAGINK")).StatusCode);
    }

    /// <summary>
    /// 所有受保護操作在未登入時回傳 401，不重新導向或執行資料驗證。
    /// </summary>
    [Theory]
    [InlineData("GET", "/api/dashboard")]
    [InlineData("PUT", "/api/account")]
    [InlineData("PUT", "/api/account/avatar")]
    [InlineData("PUT", "/api/account/password")]
    [InlineData("PUT", "/api/studio/creator")]
    [InlineData("POST", "/api/studio/works")]
    [InlineData("POST", "/api/studio/commissions")]
    [InlineData("PUT", "/api/studio/plans")]
    [InlineData("POST", "/api/studio/creators/1/follow")]
    [InlineData("POST", "/api/studio/works/1/comments")]
    [InlineData("POST", "/api/studio/works/1/like")]
    [InlineData("POST", "/api/studio/plans/1/subscribe")]
    [InlineData("POST", "/api/studio/orders")]
    [InlineData("PUT", "/api/studio/orders/1")]
    [InlineData("DELETE", "/api/studio/works/1")]
    [InlineData("POST", "/api/events")]
    [InlineData("PUT", "/api/events/1")]
    [InlineData("POST", "/api/events/1/entries")]
    [InlineData("POST", "/api/events/entries/1/like")]
    [InlineData("GET", "/api/catalog/works?following=true")]
    public async Task ProtectedEndpointsRejectAnonymousRequests(string method, string path)
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient(new() { AllowAutoRedirect = false });
        using var request = new HttpRequestMessage(new HttpMethod(method), path)
        {
            Content = method == "GET" ? null : JsonContent.Create(new { }),
        };
        var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(response.Headers.Location);
    }

    /// <summary>
    /// 註冊立即登入、切換帳號時更新身分，且無效驗證 Cookie 無法登入。
    /// </summary>
    [Fact]
    public async Task RegistrationSignsInAndLoginReplacesIdentity()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var client = factory.CreateClient();
        var registration = await client.PostAsJsonAsync("/api/account/register", new
        {
            email = "new@example.test",
            password = "password4",
            name = "New reader",
        });
        registration.EnsureSuccessStatusCode();
        Assert.Equal("new@example.test",
            (await registration.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("email").GetString());
        var cookie = Assert.Single(registration.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("Imagink.Auth=", cookie);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=lax", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("new@example.test",
            (await client.GetFromJsonAsync<JsonElement>("/api/account")).GetProperty("email").GetString());
        await Login(client);
        Assert.Equal(2,
            (await client.GetFromJsonAsync<JsonElement>("/api/account")).GetProperty("id").GetInt32());

        using var invalid = factory.CreateClient(new() { HandleCookies = false, AllowAutoRedirect = false });
        invalid.DefaultRequestHeaders.Add("Cookie", "Imagink.Auth=invalid");
        Assert.Equal(HttpStatusCode.Unauthorized, (await invalid.GetAsync("/api/dashboard")).StatusCode);
    }

    /// <summary>
    /// 驗證創作者可發布及移除作品，讀者和其他創作者受到權限限制。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task OwnerCanPublishAndRemoveWorkButReaderCannot()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var owner = factory.CreateClient();
        using var reader = factory.CreateClient();
        await Login(owner, true);
        await Login(reader);
        var post = new
        {
            title = "New work",
            description = "Body",
            categoryId = 1,
            subtitleId = 1,
            planId = 1,
            image = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
        };
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await reader.PostAsJsonAsync("/api/studio/works", post)).StatusCode
        );
        var created = await owner.PostAsJsonAsync("/api/studio/works", post);
        created.EnsureSuccessStatusCode();
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("id")
            .GetInt32();
        using var other = factory.CreateClient();
        await LoginOtherCreator(other);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Delete(other, $"/api/studio/works/{id}")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Delete(other, "/api/studio/plans/1")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.OK,
            (await reader.GetAsync($"/images/content/{id}")).StatusCode
        );
        using var delete = new HttpRequestMessage(HttpMethod.Delete, $"/api/studio/works/{id}")
        {
            Content = JsonContent.Create(new { }),
        };
        (await owner.SendAsync(delete)).EnsureSuccessStatusCode();
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await reader.GetAsync($"/images/content/{id}")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await reader.GetAsync($"/api/catalog/works/{id}")).StatusCode
        );
    }

    /// <summary>
    /// 驗證訂閱採用伺服器端的方案資料，並拒絕重複訂閱。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task SubscriptionUsesServerPlanAndRejectsDuplicate()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var reader = factory.CreateClient();
        await Login(reader);
        (
            await reader.PostAsJsonAsync(
                "/api/studio/plans/1/subscribe",
                new { creatorId = 99, userId = 1 }
            )
        ).EnsureSuccessStatusCode();
        Assert.Equal(
            HttpStatusCode.Conflict,
            (await reader.PostAsJsonAsync("/api/studio/plans/1/subscribe", new { })).StatusCode
        );
        var dashboard = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        var subscription = dashboard.GetProperty("subscriptions").EnumerateArray().Single();
        Assert.Equal(1, subscription.GetProperty("creatorId").GetInt32());
        var start = DateOnly.Parse(subscription.GetProperty("startDate").GetString()!);
        Assert.Equal(
            start.AddMonths(1).ToString("yyyy-MM-dd"),
            subscription.GetProperty("endDate").GetString()
        );
    }

    /// <summary>
    /// 驗證委託訂單狀態變更需要正確的參與者及目前狀態。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task OrderTransitionsRequireCorrectParticipantAndCurrentState()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var owner = factory.CreateClient();
        using var reader = factory.CreateClient();
        await Login(owner, true);
        await Login(reader);
        (
            await reader.PostAsJsonAsync(
                "/api/studio/orders",
                new
                {
                    commissionId = 1,
                    price = 200,
                    description = "Please draw",
                }
            )
        ).EnsureSuccessStatusCode();
        var dashboard = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        var id = dashboard.GetProperty("orders")[0].GetProperty("id").GetInt32();
        using var other = factory.CreateClient();
        await LoginOtherCreator(other);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (
                await other.PutAsJsonAsync(
                    $"/api/studio/orders/{id}",
                    new { action = "accept", price = 1 }
                )
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await reader.PutAsJsonAsync(
                    $"/api/studio/orders/{id}",
                    new { action = "accept", price = 1 }
                )
            ).StatusCode
        );
        (
            await owner.PutAsJsonAsync(
                $"/api/studio/orders/{id}",
                new { action = "accept", price = 300 }
            )
        ).EnsureSuccessStatusCode();
        (
            await reader.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })
        ).EnsureSuccessStatusCode();
        (
            await owner.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })
        ).EnsureSuccessStatusCode();
        (
            await owner.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })
        ).EnsureSuccessStatusCode();
        var result = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        Assert.Equal("完成", result.GetProperty("orders")[0].GetProperty("status").GetString());
        Assert.Equal(300, result.GetProperty("orders")[0].GetProperty("price").GetInt32());
    }

    /// <summary>
    /// 驗證活動編輯保留指定範例圖片，並檢查編輯及刪除權限。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task EventEditRetainsSelectedSamplesAndChecksOwnership()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var owner = factory.CreateClient();
        using var reader = factory.CreateClient();
        await Login(owner, true);
        await Login(reader);
        const string image = "data:image/png;base64,iVBORw0KGgo=";
        var input = new
        {
            name = "Event",
            description = "Intro",
            startDate = DateTime.Now.AddDays(-1),
            endDate = DateTime.Now.AddDays(1),
            categoryId = 1,
            banner = image,
            samples = new[] { image },
            titleColor = "#112233",
            introColor = "#ffffff",
        };
        var created = await owner.PostAsJsonAsync("/api/events", input);
        created.EnsureSuccessStatusCode();
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>())
            .GetProperty("id")
            .GetInt32();
        Assert.Equal(
            HttpStatusCode.Forbidden,
            (await reader.PutAsJsonAsync($"/api/events/{id}", input)).StatusCode
        );
        using var other = factory.CreateClient();
        await LoginOtherCreator(other);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await other.PutAsJsonAsync($"/api/events/{id}", input)).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await Delete(other, $"/api/studio/events/{id}")).StatusCode
        );
        var detail = await owner.GetFromJsonAsync<JsonElement>($"/api/events/{id}");
        var sample = detail.GetProperty("images")[0].GetProperty("id").GetInt32();
        (
            await owner.PutAsJsonAsync(
                $"/api/events/{id}",
                new
                {
                    input.name,
                    input.description,
                    input.startDate,
                    input.endDate,
                    input.categoryId,
                    samples = Array.Empty<string>(),
                    keepSamples = new[] { sample },
                    input.titleColor,
                    input.introColor,
                }
            )
        ).EnsureSuccessStatusCode();
        var updated = await owner.GetFromJsonAsync<JsonElement>($"/api/events/{id}");
        Assert.Single(updated.GetProperty("images").EnumerateArray());
        Assert.Contains(
            "#112233",
            updated.GetProperty("item").GetProperty("eventStyle").GetString()
        );
        Assert.Equal(
            HttpStatusCode.OK,
            (await reader.GetAsync($"/images/event-image/{sample}")).StatusCode
        );
        using var delete = new HttpRequestMessage(HttpMethod.Delete, $"/api/studio/events/{id}")
        {
            Content = JsonContent.Create(new { }),
        };
        (await owner.SendAsync(delete)).EnsureSuccessStatusCode();
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await reader.GetAsync($"/images/event-image/{sample}")).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await reader.GetAsync($"/images/event/{id}")).StatusCode
        );
    }

    /// <summary>
    /// 驗證不符合規則的註冊及作品資料會收到驗證錯誤。
    /// </summary>
    /// <returns>表示測試執行的非同步工作</returns>
    [Fact]
    public async Task InvalidInputReturnsValidationErrorsWithoutWriting()
    {
        using var factory = new ApiFactory();
        await factory.Seed();
        using var client = factory.CreateClient();
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await client.PostAsJsonAsync(
                    "/api/account/register",
                    new
                    {
                        email = "valid@example.test",
                        password = "password",
                        name = new string('x', 41),
                    }
                )
            ).StatusCode
        );
        await Login(client, true);
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await client.PostAsJsonAsync(
                    "/api/studio/works",
                    new
                    {
                        title = "Valid",
                        description = "Body",
                        categoryId = 999,
                        subtitleId = 1,
                        planId = 1,
                        image = "AQID",
                    }
                )
            ).StatusCode
        );
        Assert.Equal(
            HttpStatusCode.BadRequest,
            (
                await client.PostAsJsonAsync(
                    "/api/studio/works",
                    new
                    {
                        title = new string('x', 201),
                        description = "Body",
                        categoryId = 1,
                        subtitleId = 1,
                        planId = 1,
                        image = "AQID",
                    }
                )
            ).StatusCode
        );
    }
}
