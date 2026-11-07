using System.Security.Claims;
using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

/// <summary>
/// API 控制器共用基底。
/// </summary>
public abstract class ApiController(ImaginkContext database) : ControllerBase
{
    /// <summary>
    /// 資料庫內容。
    /// </summary>
    protected ImaginkContext Database { get; } = database;

    /// <summary>
    /// 目前登入使用者的識別碼。
    /// </summary>
    protected int? UserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    /// <summary>
    /// 取得目前使用者的創作者識別碼
    /// </summary>
    /// <returns>創作者識別碼</returns>
    protected Task<int?> CreatorId() =>
        Database
            .Users.Where(u => u.UserId == UserId)
            .Select(u => u.CreatorId)
            .FirstOrDefaultAsync();
}
