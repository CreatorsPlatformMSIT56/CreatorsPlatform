using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

public abstract class ApiController(ImaginkContext database) : ControllerBase
{
    protected ImaginkContext Database { get; } = database;
    protected int? UserId => HttpContext.Session.GetInt32("userId");
    protected Task<int?> CreatorId() => Database.Users.Where(u => u.UserId == UserId)
        .Select(u => u.CreatorId).FirstOrDefaultAsync();
}
