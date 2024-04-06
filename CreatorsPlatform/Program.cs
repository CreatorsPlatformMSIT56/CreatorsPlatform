using Microsoft.EntityFrameworkCore;
using CreatorsPlatform.Controllers;
using CreatorsPlatform.Models;

//使用Session資料方法:
//設置Session:HttpContext.Session.SetInt32/SetString/......(名稱, 內容);
//取得Session:HttpContext.Session.GetInt32/GetString/......(名稱, 內容);

var builder = WebApplication.CreateBuilder(args);
#region

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ImaginkContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ImaginkConnstring")
    ));

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.Name = "DefaultName";
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=yhu}/{action=IMAGINK}/{id?}"
    );

app.Run();

