using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using static CreatorsPlatform.Controllers.CreatorController.CreatorDetailsViewModel;
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
        public class ContentsModel
        {
            public int ContentId { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? UploadDate { get; set; }
            public byte[]? ImageUrl { get; set; }
            public int? Likes { get; set; }
            public int CategoryId { get; set; }
            public int? SubtitleId { get; set; }
            public string? SubtitleName { get; set; }
            public int CreatorId { get; set; }
            public int PlanId { get; set; }
        }
        public class ContentTagsModel
        {
            public string? Title { get; set; }
            public string? TagName { get; set; }
            public int TagId {  get; set; }
        }
        public class CreatorDetailsViewModel
        {
            //public string Name { get; set; } = "Error";
            //public string Email { get; set; } = "Error";
            //public string Password { get; set; } = "Error";

            public Creator? Creator { get; set; }
            public string? UserName { get; set; }
            public byte[]? UserAvatar { get; set; }
            public IEnumerable<CommissionWithImageAndWord>? CommissionsWithWords { get; set; }
            public IEnumerable<ContentsModel>? ContentsModel { get; set; }

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

            // 取得委託資料並取用第一筆資料
            var commissionsWithWords = from c in _context.CommissionWithImageAndWords
                                       where c.CreatorId == id
                                       group c by c.Title into g
                                       select g.OrderBy(x => x.CommissionId).First();

            var contents = from c in _context.Contents
                           join s in _context.Subtitles
                           on c.SubtitleId equals s.SubtitleId
                           where c.CreatorId == id
                           select new ContentsModel
                           { 
                               ContentId = c.CreatorId,
                               Title = c.Title,
                               Description = c.Description,
                               UploadDate = c.UploadDate,
                               ImageUrl = c.ImageUrl,
                               Likes = c.Likes,
                               CategoryId = c.CategoryId,
                               SubtitleId = c.SubtitleId,
                               SubtitleName = s.SubtitleName,
                               CreatorId = c.CreatorId,
                               PlanId = c.PlanId,
                           };

            


            var viewModel = new CreatorDetailsViewModel
            {
                Creator = creator,
                UserName = userName,
                UserAvatar = userAvatar,
                CommissionsWithWords = commissionsWithWords,
                ContentsModel = contents.ToList()
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
            public IEnumerable<ContentTagsModel>? ContentTagsModel { get; set; }

        }
        public IActionResult GetPost(int id)
        {
            // 取得作品資料
            var content = _context.Contents
                .Include(c => c.Creator)
                .ThenInclude(cr => cr.Users)
                .FirstOrDefault(c => c.ContentId == id);

            // 取得留言資料
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

            // 取得訂閱方案資料
            var plans = _context.Plans
                .Include(p => p.Creator)
                .ThenInclude(c => c.Users)
                .Where(p => p.Creator.CreatorId == content!.CreatorId)
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

            var contentTags = from c in _context.Contents
                              join ct in _context.ContentTags
                              on c.ContentId equals ct.ContentId
                              join t in _context.Tags
                              on ct.TagId equals t.TagId
                              where c.ContentId == id
                              select new ContentTagsModel
                              {
                                  Title = c.Title,
                                  TagName = t.TagName,
                                  TagId = t.TagId
                              };


            // 整合取得的資料
            var viewModel = new ContentDetailsViewModel
            {
                Content = new List<Content> { content },
                Plans = plans,
                Comments = comments,
                ContentTagsModel = contentTags.ToList()
            };

            return View(viewModel);
        }


		//----------------------------------------------------------------
		//委託貼文頁面
		public class CommissionDetailsViewModel
		{
			public IEnumerable<Commission>? Commission { get; set; }
			public IEnumerable<Plan>? Plans { get; set; }
			public IEnumerable<Comment>? Comments { get; set; }
            public IEnumerable<CommissionWithImageAndWord>? CommissionsWithWords { get; set; }
        }

        public IActionResult GetCommission(int id = 1)
		{
			var commission = _context.Commissions
				.Include(c => c.Creator)
				.ThenInclude(cr => cr.Users)
				.Include(c => c.Subtitle)
				.Include(c => c.CommissionImages)
				.FirstOrDefault(c => c.CommissionId == id);

			var comments = _context.Comments
				.Include(u => u.User)
				.Where(c => c.UserId == id)
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

            var commissionsWithWords = from c in _context.CommissionWithImageAndWords
                                       where c.CreatorId == id
                                       group c by c.Title into g
                                       select g.OrderBy(x => x.CommissionId).First();

            var viewModel = new CommissionDetailsViewModel
			{
				Commission = new List<Commission> { commission! },
				Plans = plans,
				Comments = comments,
                CommissionsWithWords = commissionsWithWords
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
