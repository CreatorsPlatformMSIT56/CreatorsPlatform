using CreatorsPlatform.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ImaginkContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ImaginkConnstring")));
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = "Imagink.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.Use(async (context, next) =>
{
    // JSON-only mutations prevent cross-origin HTML forms from using the session.
    if (context.Request.Path.StartsWithSegments("/api") &&
        !HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method) &&
        !context.Request.HasJsonContentType())
    {
        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
        return;
    }
    await next();
});
app.UseStaticFiles();
app.UseSession();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();

public partial class Program;
