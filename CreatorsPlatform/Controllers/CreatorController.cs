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
            public IEnumerable<CommissionWithImageAndWord>? CommissionsWithWords { get; set; }
        }
        //public class CommisionsWithWords
        //{
        //    public IEnumerable<Commission>? Comissions { get; set; }
        //    public int ComID { get; set; }
        //    public string? ComTitle { get; set; }
        //    public int ComPriceMin { get; set; }
        //    public int? ComPriceMax { get; set; }
        //    public string? ComDes { get; set; }
        //    public DateOnly ComPutUpDate { get; set; }
        //    public DateOnly? ComOverDate { get; set; }
        //    public byte[]? ImageUrl { get; set; }
        //    public string? SubtitleName { get; set; }
        //}

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

            //var commisionsWithWords = from commission in _context.Commissions
            //                        where commission.CreatorId == id
            //                        join image in _context.CommissionImages
            //                        on commission.CommissionId equals image.CommissionId
            //                        join subtitle in _context.Subtitles
            //                        on commission.SubtitleId equals subtitle.SubtitleId
            //                        select new CommisionsWithWords
            //                        {
            //                            ComID = commission.CommissionId, 
            //                            ComTitle = commission.Title,
            //                            ComPriceMin = commission.PriceMin,
            //                            ComPriceMax = commission.PriceMax,
            //                            ComDes = commission.Description,
            //                            ComPutUpDate = commission.PutUpDate,
            //                            ComOverDate = commission.OverDate,
            //                            ImageUrl = image.ImageUrl,
            //                            SubtitleName = subtitle.SubtitleName
            //                        };
            //var commisionsWithWords = _context.CommissionWithImageAndWords.Where(c => c.CreatorId == id);

            var commissionsWithWords = from c in _context.CommissionWithImageAndWords
                                       where c.CreatorId == id
                                       select c;



            var viewModel = new CreatorDetailsViewModel
            {
                Creator = creator,
                UserName = userName,
                UserAvatar = userAvatar,
                Commissions = commisions,
                CommissionsWithWords = commissionsWithWords
            };

            return View(viewModel);
        }

        //這是測試
        public IActionResult Test(int id)
        {
            //假設這是PlanId
            var p = (from c in _context.Plans
                    where c.PlanId == id
                    select c).ToList();
            
            var query = _context.Plans.ToList();
            return View(p);
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
