using CreatorsPlatform.Models;
using Newtonsoft.Json;
using static CreatorsPlatform.Controllers.HomeController;


namespace CreatorsPlatform.CsMod.yhu
{
    public class LinqFunctionReserve 
    {
        private readonly ImaginkContext _context;

        public LinqFunctionReserve(ImaginkContext context)
        {
            _context = context;
        }
        public int GetUserid(MemberData member)
        {
            var UserId = (from UserIdData in _context.Users
                              where UserIdData.Email == member.Email
                              select new { UserIdData.UserId }).ToList();
            return UserId[0].UserId;
        }
    }
}
