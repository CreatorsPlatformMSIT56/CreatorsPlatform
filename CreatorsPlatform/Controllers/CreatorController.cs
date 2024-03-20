using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using static CreatorsPlatform.Controllers.HomeController;

namespace CreatorsPlatform.Controllers
{
    public class CreatorController : Controller
    {
        


        // 導入Context
        private readonly ImaginkContext _context;

        public CreatorController(ImaginkContext context)
        {
            _context = context;
        }

        public class CreatorDetailsViewModel
        {
            //public string Name { get; set; } = "Error";
            //public string Email { get; set; } = "Error";
            //public string Password { get; set; } = "Error";


            public Creator? Creator { get; set; }
            public string? UserName { get; set; }
            public byte[]? UserAvatar { get; set; }
            public IEnumerable<Commission>? Commissions { get; set; }
        }

        // 創作者首頁
        public IActionResult Index(int id)
        {
            //var memberJson = HttpContext.Session.GetString("key");
            //MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var creator = _context.Creators
                .Include(c => c.Users)
                .FirstOrDefault(c => c.CreatorId == id);

            var userName = creator?.Users.FirstOrDefault()?.UserName;

            var userAvatar = creator?.Users.FirstOrDefault()?.Avatar;

            var commisions = _context.Commissions.Where(c => c.CreatorId == id);

            var viewModel = new CreatorDetailsViewModel
            {
                Creator = creator,
                UserName = userName,
                UserAvatar = userAvatar,
                Commissions = commisions
            };

            return View(viewModel);
        }

        // 創作者建立貼文(修改位置待訂)
        public IActionResult AddPost()
        {

            return View();
        }

        // 創作者貼文頁面
        public IActionResult GetPost()
        {
            return View();
        }

        // 創作者建立接受委託表單(修改位置待訂)
        public IActionResult AddCommisionForm()
        {
            return View();
        }

        // 創作者編輯訂閱方案
        public IActionResult EditSubscriptionPlans()
        {
            return View();
        }
    }
}
