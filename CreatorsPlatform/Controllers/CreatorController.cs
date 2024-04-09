using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Buffers.Text;
using System.Collections;
using static CreatorsPlatform.Controllers.CreatorController.CreatorDetailsViewModel;
using static CreatorsPlatform.Controllers.yhuController;

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
        //臨時增加
        public string MembersIcon(int x)
        {
            var MembersIcon = (from UserData in _context.Users
                               where UserData.UserId == x
                               select UserData.Avatar).FirstOrDefault();
            string Avatar = Convert.ToBase64String(MembersIcon);
            return Avatar;
        }
        public bool MembersOnline()
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //

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
            public int TagId { get; set; }
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
                               ContentId = c.ContentId,
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

            // 傳訂閱層級(Plan)到前端
            var CreatorPlan = _context.Plans.Where(e => e.CreatorId == id).ToList();
            ViewBag.CreatorPlan = CreatorPlan;

            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();

                // 如果有登入，傳登入的資料過去
                ViewBag.UserId = member.id;
                ViewBag.CreatorId = id;
                // 如果有登入，把檢查是否追蹤結果傳到前端
                var Follow = _context.Follows.Any(f => f.UserId == member.id && f.CreatorId == id);
                if (Follow)
                {
                    var TheFollow = _context.Follows.FirstOrDefault(f => f.UserId == member.id && f.CreatorId == id).Unfollow;
                    ViewBag.UnFollow = TheFollow;
                }
                else
                {
                    ViewBag.UnFollow = true;
                }
            }
            else
            {
                ViewBag.UnFollow = true;
                ViewBag.CreatorId = id;
                ViewBag.MembersOnline = MembersOnline();
            };
            //
            return View(viewModel);
        }
        // 關注功能
        [HttpPost]
        public string FollowCreator(int TheUserId, int TheCreatorId)
        {
            // 如果已經存在資料表裡
            var result = _context.Follows.Any(e => e.UserId == TheUserId && e.CreatorId == TheCreatorId);
            if (result)
            {
                // 取關就關注，關注就取關
                if (_context.Follows.Any(e => e.UserId == TheUserId && e.CreatorId == TheCreatorId && e.Unfollow == true))
                {
                    var TheFollow = _context.Follows.FirstOrDefault(e => e.UserId == TheUserId && e.CreatorId == TheCreatorId && e.Unfollow == true);
                    TheFollow.Unfollow = false;
                    _context.Follows.Update(TheFollow);
                }
                else
                {
                    var TheFollow = _context.Follows.FirstOrDefault(e => e.UserId == TheUserId && e.CreatorId == TheCreatorId && e.Unfollow == false);
                    TheFollow.Unfollow = true;
                    _context.Follows.Update(TheFollow);
                }
            }
            // 沒有就new一個加關注
            else
            {
                Follow NewFollow = new Follow
                {
                    UserId = TheUserId,
                    CreatorId = TheCreatorId,
                    Unfollow = false
                };
                _context.Follows.Add(NewFollow);
            }
            _context.SaveChanges();
            return "OK";
        }


        // 創作者建立貼文(修改位置待訂)
        [HttpGet]
        public async Task<IActionResult> AddPost()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            //
            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
            };
            //
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(string Title, int? SubtitleId, string CategoryIdstring, string Description, string Imagebase64)
        {
			// 取得 member 對應之 creatorId
			var memberJson = HttpContext.Session.GetString("key");
			MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
			int NowCreatorId = (int)_context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;

            // 發布時間在這邊取得並轉換成資料庫要的型態
            var sDate = DateTime.Now;
            DateOnly ssDate = DateOnly.FromDateTime(sDate);

			if (ModelState.IsValid != null)
			{
                byte[] imagebinaryData = Convert.FromBase64String(Imagebase64);

				var Newcontent = new Content
				{
					Title = Title,
					Description = Description,
					ImageUrl = imagebinaryData,
					UploadDate = sDate,
					CategoryId = Convert.ToInt32(CategoryIdstring),
                    Likes = 0,
					SubtitleId = SubtitleId,
					CreatorId = NowCreatorId,
					/*PlanId = content.PlanId,*/ // AddPost沒放
					PlanId = 3, // AddPost沒放 先隨便放

				};

				// 存 Content 進DB
				_context.Contents.Add(Newcontent);
				await _context.SaveChangesAsync();

				//return Ok();
				return RedirectToAction("Index", "Creator");
			}
			//ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
			//
			if (MembersOnline())
			{
				//var memberJson = HttpContext.Session.GetString("key");
				//MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
				ViewBag.MembersIcon = MembersIcon(member.id);
				ViewBag.MembersOnline = MembersOnline();
			}
			else
			{
				ViewBag.MembersOnline = MembersOnline();
			};
			// return RedirectToAction("Index");
			// return View(content);
			return Ok();
		}

        // 創作者貼文頁面
        public class ContentDetailsViewModel
        {
            public IEnumerable<Content>? Content { get; set; }
            public IEnumerable<Plan>? Plans { get; set; }
            public IEnumerable<Comment>? Comments { get; set; }
            public IEnumerable<ContentTagsModel>? ContentTagsModel { get; set; }
            public IEnumerable<ContentsModel>? Contents { get; set; }

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

            // 取得作品Tags
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

            // 取得創作者其他作品
            var contents = from c in _context.Contents
                           join s in _context.Subtitles
                           on c.SubtitleId equals s.SubtitleId
                           where c.CreatorId == content.CreatorId
                           select new ContentsModel
                           {
                               ContentId = c.ContentId,
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

            // 整合取得的資料
            var viewModel = new ContentDetailsViewModel
            {
                Content = new List<Content> { content },
                Plans = plans,
                Comments = comments,
                ContentTagsModel = contentTags.ToList(),
                Contents = contents.ToList()
            };
            //
            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();

                ViewBag.UserId = member.id;
                // 如果有登入，把檢查是否追蹤結果傳到前端
                var Follow = _context.Follows.Any(f => f.UserId == member.id && f.CreatorId == id);
                if (Follow)
                {
                    var TheFollow = _context.Follows.FirstOrDefault(f => f.UserId == member.id && f.CreatorId == id).Unfollow;
                    ViewBag.UnFollow = TheFollow;
                }
                else
                {
                    ViewBag.UnFollow = true;
                }

                //判斷用戶是否具備足夠權限觀看
                //DateOnly currentDateOnly = DateOnly.FromDateTime(DateTime.Today);
                //var SubOrNot = _context.Subscriptions.Any(e => e.UserId == member.id && e.CreatorId == content.CreatorId && e.EndDate > currentDateOnly);
                //var SubLevelHigh = _context.Subscriptions.FirstOrDefault(e => e.UserId == member.id && e.CreatorId == content.CreatorId && e.EndDate > currentDateOnly).PlanId;
                //var UserPlanLevel = _context.Plans.FirstOrDefault(e => e.PlanId == SubLevelHigh).PlanLevel;
                //var NowPostPlanLevel = _context.Plans.FirstOrDefault(e => e.PlanId == content.PlanId).PlanLevel;
                //if (SubOrNot || NowPostPlanLevel> UserPlanLevel)
                //{
                //    return View("Index");
                //}
            }
            else
            {
                //判斷用戶是否具備足夠權限觀看
                //if (content.PlanId != null)
                //{
                //    return View("Index");
                //}

                ViewBag.MembersOnline = MembersOnline();
                ViewBag.UserId = 0;
                ViewBag.UnFollow = true;
            };
            //
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

            var commissionsWithWords = (from c in _context.CommissionWithImageAndWords
                                        where c.CreatorId == id
                                        group c by c.Title into g
                                        select g.OrderBy(x => x.CommissionId).First()).Take(3); // 只取三個

            var viewModel = new CommissionDetailsViewModel
            {
                Commission = new List<Commission> { commission! },
                Plans = plans,
                Comments = comments,
                CommissionsWithWords = commissionsWithWords
            };
            //
            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();


                // 如果有登入，傳登入的資料過去
                ViewBag.CreatorId = commission.CreatorId;
                ViewBag.UserId = member.id;
                // 傳是否有追蹤過去
                var Follow = _context.Follows.Any(f => f.UserId == member.id && f.CreatorId == id);
                if (Follow)
                {
                    var TheFollow = _context.Follows.FirstOrDefault(f => f.UserId == member.id && f.CreatorId == id).Unfollow;
                    ViewBag.UnFollow = TheFollow;
                }
                else
                {
                    ViewBag.UnFollow = true;
                }
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
                ViewBag.UserId = 0;
                ViewBag.UnFollow = true;
                ViewBag.CreatorId = commission.CreatorId;
            };
            //
            return View(viewModel);
        }


        // 創作者建立接受委託表單(修改位置待訂)
        public IActionResult AddCommisionForm()
        {
            //
            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
            };
            //
            return View();
        }

        // 新稱委託
        [HttpPost]
        public IActionResult CommissionCreate(Commission CreateCommissionModel)
        {
            // 取得 member 對應之 creatorId
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            int NowCreatorId = (int)_context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;

            // 發布時間在這邊取得並轉換成資料庫要的型態
            var sDate = DateTime.Now;
            DateOnly ssDate = DateOnly.FromDateTime(sDate);


            var CreateCommission = new Commission
            {
                Title = CreateCommissionModel.Title,
                PriceMin = CreateCommissionModel.PriceMin,
                PriceMax = CreateCommissionModel.PriceMax,
                CreatorId = NowCreatorId,
                Description = CreateCommissionModel.Description,
                PutUpDate = ssDate,
                OverDate = CreateCommissionModel.OverDate,
                SubtitleId = CreateCommissionModel.SubtitleId
            };

            if (ModelState.IsValid != null)
            {
                _context.Add(CreateCommission);
                _context.SaveChanges();

                // 拿到對應新委託Id 新增範例圖片要用
                TempData["TheNewCommissionID"] = CreateCommission.CommissionId;
                return Ok();
            }

            return BadRequest();

        }

        // 新增範例圖片
        [HttpPost]
        public IActionResult CreateCommissionExImg(CommissionImage commissionImage)
        {
            int newCommissionId = Convert.ToInt32(TempData["TheNewCommissionID"]);

            var CommissionImageToDB = new CommissionImage
            {
                ImageUrl = commissionImage.ImageUrl,
                CommissionId = newCommissionId
            };

            _context.Add(CommissionImageToDB);
            _context.SaveChanges();

            return Ok();
        }


		// 創作者編輯訂閱方案
		public IActionResult EditSubscriptionPlans()
		{
			//
			if (MembersOnline())
			{
				var memberJson = HttpContext.Session.GetString("key");
				MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
				ViewBag.MembersIcon = MembersIcon(member.id);
				ViewBag.MembersOnline = MembersOnline();
			}
			else
			{
				ViewBag.MembersOnline = MembersOnline();
			};
			//
			return View();
		}
		public IActionResult Test()
		{
			return View();
		}
		[HttpPost]
		public IActionResult TestAjax() { 
			return Ok();
		}

	}
}
