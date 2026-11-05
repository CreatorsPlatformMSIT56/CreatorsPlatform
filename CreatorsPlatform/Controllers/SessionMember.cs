using CreatorsPlatform.Models;
using static CreatorsPlatform.Controllers.yhuController;

namespace CreatorsPlatform.Controllers;

internal static class SessionMember
{
    public static MemberData? Get(HttpContext context, ImaginkContext database)
    {
        context.Session.Remove("key");
        if (!context.Session.TryGetValue("userId", out var data) || data.Length != sizeof(int)) return null;
        var id = context.Session.GetInt32("userId");
        if (id is null or <= 0) return null;

        return database.Users.Where(user => user.UserId == id)
            .Select(user => new MemberData { id = user.UserId, Name = user.UserName, Email = user.Email })
            .FirstOrDefault();
    }
}
