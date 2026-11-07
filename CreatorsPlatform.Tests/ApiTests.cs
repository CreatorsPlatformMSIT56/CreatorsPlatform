using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace CreatorsPlatform.Tests;

public class ApiTests
{
    private static async Task Login(HttpClient client, bool owner = false)
    {
        var response = await client.PostAsJsonAsync("/api/account/login", new
        {
            email = owner ? "owner@example.test" : "reader@example.test", password = owner ? "password1" : "password2"
        });
        response.EnsureSuccessStatusCode();
    }

    private static async Task LoginOtherCreator(HttpClient client) =>
        (await client.PostAsJsonAsync("/api/account/login", new { email = "other@example.test", password = "password3" })).EnsureSuccessStatusCode();

    private static Task<HttpResponseMessage> Delete(HttpClient client, string path) =>
        client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, path) { Content = JsonContent.Create(new { }) });

    [Fact]
    public async Task SessionRoundtripDoesNotExposePassword()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var client = factory.CreateClient();
        await Login(client);
        var response = await client.GetAsync("/api/account");
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("reader@example.test", body);
        Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(2, JsonDocument.Parse(body).RootElement.GetProperty("id").GetInt32());
        (await client.PostAsJsonAsync("/api/account/logout", new { })).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/dashboard")).StatusCode);
    }

    [Fact]
    public async Task MutationRequiresJsonAndAuthenticatedSession()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, (await client.PostAsync("/api/studio/works/1/like", new FormUrlEncodedContent([]))).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/studio/works/1/like", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/yhu/IMAGINK")).StatusCode);
    }

    [Fact]
    public async Task OwnerCanPublishAndRemoveWorkButReaderCannot()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var owner = factory.CreateClient(); using var reader = factory.CreateClient();
        await Login(owner, true); await Login(reader);
        var post = new { title = "New work", description = "Body", categoryId = 1, subtitleId = 1, planId = 1, image = Convert.ToBase64String(new byte[] { 1, 2, 3 }) };
        Assert.Equal(HttpStatusCode.Forbidden, (await reader.PostAsJsonAsync("/api/studio/works", post)).StatusCode);
        var created = await owner.PostAsJsonAsync("/api/studio/works", post); created.EnsureSuccessStatusCode();
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();
        using var other = factory.CreateClient(); await LoginOtherCreator(other);
        Assert.Equal(HttpStatusCode.NotFound, (await Delete(other, $"/api/studio/works/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Delete(other, "/api/studio/plans/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await reader.GetAsync($"/images/content/{id}")).StatusCode);
        using var delete = new HttpRequestMessage(HttpMethod.Delete, $"/api/studio/works/{id}") { Content = JsonContent.Create(new { }) };
        (await owner.SendAsync(delete)).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NotFound, (await reader.GetAsync($"/images/content/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await reader.GetAsync($"/api/catalog/works/{id}")).StatusCode);
    }

    [Fact]
    public async Task SubscriptionUsesServerPlanAndRejectsDuplicate()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var reader = factory.CreateClient(); await Login(reader);
        (await reader.PostAsJsonAsync("/api/studio/plans/1/subscribe", new { creatorId = 99, userId = 1 })).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Conflict, (await reader.PostAsJsonAsync("/api/studio/plans/1/subscribe", new { })).StatusCode);
        var dashboard = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        var subscription = dashboard.GetProperty("subscriptions").EnumerateArray().Single();
        Assert.Equal(1, subscription.GetProperty("creatorId").GetInt32());
        var start = DateOnly.Parse(subscription.GetProperty("startDate").GetString()!);
        Assert.Equal(start.AddMonths(1).ToString("yyyy-MM-dd"), subscription.GetProperty("endDate").GetString());
    }

    [Fact]
    public async Task OrderTransitionsRequireCorrectParticipantAndCurrentState()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var owner = factory.CreateClient(); using var reader = factory.CreateClient();
        await Login(owner, true); await Login(reader);
        (await reader.PostAsJsonAsync("/api/studio/orders", new { commissionId = 1, price = 200, description = "Please draw" })).EnsureSuccessStatusCode();
        var dashboard = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        var id = dashboard.GetProperty("orders")[0].GetProperty("id").GetInt32();
        using var other = factory.CreateClient(); await LoginOtherCreator(other);
        Assert.Equal(HttpStatusCode.NotFound, (await other.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept", price = 1 })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await reader.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept", price = 1 })).StatusCode);
        (await owner.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept", price = 300 })).EnsureSuccessStatusCode();
        (await reader.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })).EnsureSuccessStatusCode();
        (await owner.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })).EnsureSuccessStatusCode();
        (await owner.PutAsJsonAsync($"/api/studio/orders/{id}", new { action = "accept" })).EnsureSuccessStatusCode();
        var result = await reader.GetFromJsonAsync<JsonElement>("/api/dashboard");
        Assert.Equal("完成", result.GetProperty("orders")[0].GetProperty("status").GetString());
        Assert.Equal(300, result.GetProperty("orders")[0].GetProperty("price").GetInt32());
    }

    [Fact]
    public async Task EventEditRetainsSelectedSamplesAndChecksOwnership()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var owner = factory.CreateClient(); using var reader = factory.CreateClient();
        await Login(owner, true); await Login(reader);
        const string image = "data:image/png;base64,iVBORw0KGgo=";
        var input = new { name = "Event", description = "Intro", startDate = DateTime.Now.AddDays(-1), endDate = DateTime.Now.AddDays(1), categoryId = 1, banner = image, samples = new[] { image }, titleColor = "#112233", introColor = "#ffffff" };
        var created = await owner.PostAsJsonAsync("/api/events", input); created.EnsureSuccessStatusCode();
        var id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();
        Assert.Equal(HttpStatusCode.Forbidden, (await reader.PutAsJsonAsync($"/api/events/{id}", input)).StatusCode);
        using var other = factory.CreateClient(); await LoginOtherCreator(other);
        Assert.Equal(HttpStatusCode.NotFound, (await other.PutAsJsonAsync($"/api/events/{id}", input)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Delete(other, $"/api/studio/events/{id}")).StatusCode);
        var detail = await owner.GetFromJsonAsync<JsonElement>($"/api/events/{id}");
        var sample = detail.GetProperty("images")[0].GetProperty("id").GetInt32();
        (await owner.PutAsJsonAsync($"/api/events/{id}", new { input.name, input.description, input.startDate, input.endDate, input.categoryId, samples = Array.Empty<string>(), keepSamples = new[] { sample }, input.titleColor, input.introColor })).EnsureSuccessStatusCode();
        var updated = await owner.GetFromJsonAsync<JsonElement>($"/api/events/{id}");
        Assert.Single(updated.GetProperty("images").EnumerateArray());
        Assert.Contains("#112233", updated.GetProperty("item").GetProperty("eventStyle").GetString());
        Assert.Equal(HttpStatusCode.OK, (await reader.GetAsync($"/images/event-image/{sample}")).StatusCode);
        using var delete = new HttpRequestMessage(HttpMethod.Delete, $"/api/studio/events/{id}") { Content = JsonContent.Create(new { }) };
        (await owner.SendAsync(delete)).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NotFound, (await reader.GetAsync($"/images/event-image/{sample}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await reader.GetAsync($"/images/event/{id}")).StatusCode);
    }

    [Fact]
    public async Task InvalidInputReturnsValidationErrorsWithoutWriting()
    {
        using var factory = new ApiFactory(); await factory.Seed();
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/account/register", new { email = "valid@example.test", password = "password", name = new string('x', 41) })).StatusCode);
        await Login(client, true);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/studio/works", new { title = "Valid", description = "Body", categoryId = 999, subtitleId = 1, planId = 1, image = "AQID" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/studio/works", new { title = new string('x', 201), description = "Body", categoryId = 1, subtitleId = 1, planId = 1, image = "AQID" })).StatusCode);
    }
}
