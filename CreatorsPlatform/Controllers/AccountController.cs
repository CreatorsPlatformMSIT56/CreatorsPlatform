using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Security.Claims;
using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CreatorsPlatform.Controllers;

/// <summary>
/// 使用者帳戶 API 控制器。
/// </summary>
[ApiController, Route("api/account")]
public class AccountController(
    ImaginkContext database,
    IMemoryCache cache,
    IConfiguration configuration
) : ApiController(database)
{
    /// <summary>
    /// 登入憑證。
    /// </summary>
    /// <param name="Email">電子郵件</param>
    /// <param name="Password">密碼</param>
    public record Credentials(
        [Required, EmailAddress, StringLength(255)] string Email,
        [Required] string Password
    );

    /// <summary>
    /// 使用者註冊資料。
    /// </summary>
    /// <param name="Email">電子郵件</param>
    /// <param name="Password">密碼</param>
    /// <param name="Name">名稱</param>
    public record Registration(
        [Required, EmailAddress, StringLength(255)] string Email,
        [Required, StringLength(100, MinimumLength = 8)] string Password,
        [Required, StringLength(40)] string Name
    );

    /// <summary>
    /// 個人資料。
    /// </summary>
    /// <param name="Email">電子郵件</param>
    /// <param name="Name">名稱</param>
    public record Profile(
        [Required, EmailAddress, StringLength(255)] string Email,
        [Required, StringLength(40)] string Name
    );

    /// <summary>
    /// 大頭貼資料。
    /// </summary>
    /// <param name="Data">圖片資料</param>
    public record AvatarData([Required] string Data);

    /// <summary>
    /// 密碼變更資料。
    /// </summary>
    /// <param name="CurrentPassword">目前密碼</param>
    /// <param name="Password">密碼</param>
    public record PasswordData(
        [Required] string CurrentPassword,
        [Required, StringLength(100, MinimumLength = 8)] string Password
    );

    /// <summary>
    /// 密碼重設請求。
    /// </summary>
    /// <param name="Email">電子郵件</param>
    public record ResetRequest([Required, EmailAddress, StringLength(255)] string Email);

    /// <summary>
    /// 密碼重設資料。
    /// </summary>
    /// <param name="Email">電子郵件</param>
    /// <param name="Code">驗證碼</param>
    /// <param name="Password">密碼</param>
    public record ResetData(
        [Required, EmailAddress, StringLength(255)] string Email,
        [Required] string Code,
        [Required, StringLength(100, MinimumLength = 8)] string Password
    );

    /// <summary>
    /// 取得目前登入的使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
    [HttpGet]
    public async Task<IActionResult> Current()
    {
        var user = await Database
            .Users.Where(u => u.UserId == UserId)
            .Select(u => new
            {
                id = u.UserId,
                name = u.UserName,
                u.Email,
                u.CreatorId,
                avatar = u.Avatar == null ? null : "/images/avatar/" + u.UserId,
            })
            .FirstOrDefaultAsync();
        return Ok(user);
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="input">使用者登入的憑證</param>
    /// <returns>登入結果</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(Credentials input)
    {
        var user = await Database.Users.FirstOrDefaultAsync(u =>
            u.Email == input.Email && u.Password == input.Password
        );
        if (user == null)
            return Unauthorized(new { message = "電子郵件或密碼錯誤。" });
        user.LastLoginDate = DateTime.Now;
        await Database.SaveChangesAsync();
        await SignInUser(user.UserId);
        return await Current();
    }

    /// <summary>
    /// 註冊使用者並登入
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register(Registration input)
    {
        if (await Database.Users.AnyAsync(u => u.Email == input.Email || u.UserName == input.Name))
            return Conflict(new { message = "電子郵件或名稱已被使用。" });
        var user = new User
        {
            Email = input.Email,
            UserName = input.Name,
            Password = input.Password,
            RegisterDate = DateTime.Now,
        };
        Database.Users.Add(user);
        await Database.SaveChangesAsync();
        await SignInUser(user.UserId);
        return await Current();
    }

    /// <summary>
    /// 登出目前使用者
    /// </summary>
    /// <returns>處理結果</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    /// <summary>
    /// 建立使用者身分並簽發驗證 Cookie。
    /// </summary>
    /// <param name="userId">使用者識別碼</param>
    private async Task SignInUser(int userId)
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        HttpContext.User = principal;
    }

    /// <summary>
    /// 更新目前使用者的個人資料
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update(Profile input)
    {
        var user = await Database.Users.FindAsync(UserId);
        if (user == null)
            return Unauthorized();
        if (
            await Database.Users.AnyAsync(u =>
                u.UserId != UserId && (u.Email == input.Email || u.UserName == input.Name)
            )
        )
            return Conflict(new { message = "電子郵件或名稱已被使用。" });
        user.Email = input.Email;
        user.UserName = input.Name;
        await Database.SaveChangesAsync();
        return await Current();
    }

    /// <summary>
    /// 更新目前使用者的大頭貼
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [Authorize]
    [HttpPut("avatar")]
    public async Task<IActionResult> Avatar(AvatarData input)
    {
        var user = await Database.Users.FindAsync(UserId);
        if (user == null)
            return Unauthorized();
        try
        {
            user.Avatar = Convert.FromBase64String(input.Data);
        }
        catch (FormatException)
        {
            return BadRequest();
        }
        if (user.Avatar.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "圖片上限為 5 MB。" });
        await Database.SaveChangesAsync();
        return await Current();
    }

    /// <summary>
    /// 變更目前使用者的密碼
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> Password(PasswordData input)
    {
        var user = await Database.Users.FindAsync(UserId);
        if (user == null || user.Password != input.CurrentPassword)
            return BadRequest(new { message = "目前密碼錯誤。" });
        user.Password = input.Password;
        await Database.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// 寄送密碼重設驗證碼
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("reset-code")]
    public async Task<IActionResult> SendCode(ResetRequest input)
    {
        if (
            string.IsNullOrEmpty(configuration["Mail:Username"])
            || string.IsNullOrEmpty(configuration["Mail:Password"])
        )
            return StatusCode(503, new { message = "尚未設定郵件服務。" });
        var key = "reset:" + input.Email.ToLowerInvariant();
        if (cache.TryGetValue(key, out _))
            return StatusCode(429, new { message = "請稍後再試。" });
        if (await Database.Users.AnyAsync(u => u.Email == input.Email))
        {
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            using var message = new MailMessage(
                configuration["Mail:Username"]!,
                input.Email,
                "IMAGINK 密碼重設",
                $"驗證碼：{code}，十分鐘內有效。"
            );
            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    configuration["Mail:Username"],
                    configuration["Mail:Password"]
                ),
                EnableSsl = true,
            };
            await smtp.SendMailAsync(message);
            cache.Set(key, (code, 0), TimeSpan.FromMinutes(10));
        }
        return Ok(new { message = "若帳號存在，驗證碼已寄出。" });
    }

    /// <summary>
    /// 驗證重設碼並更新密碼
    /// </summary>
    /// <param name="input">請求資料</param>
    /// <returns>處理結果</returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset(ResetData input)
    {
        var key = "reset:" + input.Email.ToLowerInvariant();
        if (!cache.TryGetValue<(string Code, int Attempts)>(key, out var token))
            return BadRequest();
        cache.Remove(key);
        if (token.Code != input.Code)
        {
            if (token.Attempts < 4)
                cache.Set(key, (token.Code, token.Attempts + 1), TimeSpan.FromMinutes(1));
            return BadRequest(new { message = "驗證碼錯誤或已失效。" });
        }
        var user = await Database.Users.FirstOrDefaultAsync(u => u.Email == input.Email);
        if (user == null)
            return BadRequest();
        user.Password = input.Password;
        await Database.SaveChangesAsync();
        return NoContent();
    }
}
