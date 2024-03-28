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
        // 創作者首頁
        public class CreatorDetailsViewModel
        {
            //public string Name { get; set; } = "Error";
            //public string Email { get; set; } = "Error";
            //public string Password { get; set; } = "Error";

            public Creator? Creator { get; set; }
            public string? UserName { get; set; }
            public byte[]? UserAvatar { get; set; }
            public IEnumerable<CommissionWithImageAndWord>? CommissionsWithWords { get; set; }
        }
        public IActionResult Index(int id)
        {
            //var memberJson = HttpContext.Session.GetString("key");
            //MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var creator = _context.Creators
                .Include(c => c.Users)
                .FirstOrDefault(c => c.CreatorId == id);

            var userName = creator?.Users.FirstOrDefault()?.UserName;

            var userAvatar = creator?.Users.FirstOrDefault()?.Avatar;

            var commissionsWithWords = from c in _context.CommissionWithImageAndWords
                                       where c.CreatorId == id
                                       group c by c.Title into g
                                       select g.OrderBy(x => x.CommissionId).First();

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

            //var commissionsWithWords = from c in _context.CommissionWithImageAndWords
            //                           where c.CreatorId == id
            //                           select c;

            var viewModel = new CreatorDetailsViewModel
            {
                Creator = creator,
                UserName = userName,
                UserAvatar = userAvatar,
                CommissionsWithWords = commissionsWithWords
            };

            return View(viewModel);
        }

        // 創作者建立貼文(修改位置待訂)
        public IActionResult AddPost()
        {

            return View();
        }

        // 創作者貼文頁面
        public class ContentDetailsViewModel
        {
            public IEnumerable<Content>? Content { get; set; }
            public IEnumerable<Plan>? Plans { get; set; }
            public IEnumerable<Comment>? Comments { get; set; }
        }

        public IActionResult GetPost(int id)
        {
            var content = _context.Contents
                .Include(c => c.Creator)
                .ThenInclude(cr => cr.Users)
                .FirstOrDefault(c => c.ContentId == id);

            var comments = _context.Comments
                .Include(u => u.User)
                .Where(c => c.ContentId == id)
                .Select(cm => new Comment
                {
                    Comment1 = cm.Comment1,
                    ContentId = cm.CommentId,
                    User = new User
                    {
                        UserId = cm.UserId,
                        UserName = cm.User.UserName,
                        Avatar = cm.User.Avatar
                    }
                }).ToList();

            var plans = _context.Plans
                .Include(p => p.Creator)
                .ThenInclude(c => c.Users)
                .Select(p => new Plan
                {
                    PlanId = p.PlanId,
                    CreatorId = p.CreatorId,
                    PlanName = p.PlanName,
                    PlanPrice = p.PlanPrice,
                    PlanLevel = p.PlanLevel,
                    Creator = new Creator
                    {
                        CreatorId = p.Creator.CreatorId,
                        Users = p.Creator.Users.Select(u => new User
                        {
                            UserId = u.UserId,
                            UserName = u.UserName,
                            Avatar = u.Avatar
                        }).ToList()
                    }
                }).ToList();
            
            
            var viewModel = new ContentDetailsViewModel
            {
                Content = new List<Content> { content },
                Plans = plans,
                Comments = comments
            };

            return View(viewModel);
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
