using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            public Creator Creator { get; set; }
            public IEnumerable<Commission> Commissions { get; set; }
            public byte[] UserAvatar { get; set; }
            public string UserName { get; set; }
        }

        // 創作者首頁
        public IActionResult Index(int id)
        {
            var creator = _context.Creators
                .Include(c => c.Users)
                .FirstOrDefault(c => c.CreatorId == id);

            var commision = _context.Commissions
                .FirstOrDefault(c => c.CreatorId == id);

            var userName = creator?.Users.FirstOrDefault()?.UserName;

            var avatar = creator?.Users.FirstOrDefault()?.Avatar;

            var viewModel = new CreatorDetailsViewModel
            {
                UserName = userName,
                Creator = creator,
                Commissions = _context.Commissions.Where(c => c.CreatorId == id),
                UserAvatar = avatar
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
