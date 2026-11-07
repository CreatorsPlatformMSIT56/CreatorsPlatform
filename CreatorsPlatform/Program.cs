using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ImaginkContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ImaginkConnstring"))
);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "Imagink.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.Use(
    async (context, next) =>
    {
        // JSON-only mutations prevent cross-origin HTML forms from using the authentication cookie.
        if (
            context.Request.Path.StartsWithSegments("/api")
            && !HttpMethods.IsGet(context.Request.Method)
            && !HttpMethods.IsHead(context.Request.Method)
            && !context.Request.HasJsonContentType()
        )
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            return;
        }
        await next();
    }
);
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();

/// <summary>
/// 應用程式進入點。
/// </summary>
public partial class Program;
