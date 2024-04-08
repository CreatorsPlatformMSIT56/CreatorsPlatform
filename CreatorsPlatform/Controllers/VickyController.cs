using Microsoft.AspNetCore.Mvc;
using CreatorsPlatform.CsMod.Vicky;
using CreatorsPlatform.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json;
using static CreatorsPlatform.Controllers.yhuController;


namespace CreatorsPlatform.Controllers
{
	public class VickyController : Controller
	{
		private readonly ImaginkContext _context;

		public VickyController(ImaginkContext context)
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

        [HttpGet]
		public IActionResult Search(string searchKey, int? subtitle, int page = 1, int pageSize = 3)
		{
			var workList = GetWorkList(searchKey, subtitle, page, pageSize);

			var totalItems = workList.Count;
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			var pagedWorkList = workList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			var viewModel = new WorkListViewModel
			{
				WorkList = pagedWorkList,
				PageNumber = page,
				TotalPages = totalPages,
				PageSize = pageSize,
				SearchKey = searchKey,
			};

			TempData["SearchKey"] = searchKey;
			ViewBag.subtitle = subtitle;



			ViewBag.subtitle_1 = CalculateSubtitleCount(1, searchKey);
			ViewBag.subtitle_2 = CalculateSubtitleCount(2, searchKey);
			ViewBag.subtitle_3 = CalculateSubtitleCount(3, searchKey);
			ViewBag.subtitle_4 = CalculateSubtitleCount(4, searchKey);
			ViewBag.subtitle_5 = CalculateSubtitleCount(5, searchKey);
			ViewBag.subtitle_6 = CalculateSubtitleCount(6, searchKey);
			ViewBag.subtitle_7 = CalculateSubtitleCount(7, searchKey);
			ViewBag.subtitle_8 = CalculateSubtitleCount(8, searchKey);
			ViewBag.subtitle_9 = CalculateSubtitleCount(9, searchKey);
			ViewBag.subtitle_10 = CalculateSubtitleCount(10, searchKey);
			ViewBag.subtitle_11 = CalculateSubtitleCount(11, searchKey);
			ViewBag.subtitle_12 = CalculateSubtitleCount(12, searchKey);
			ViewBag.subtitle_13 = CalculateSubtitleCount(13, searchKey);
			ViewBag.subtitle_14 = CalculateSubtitleCount(14, searchKey);
			ViewBag.subtitle_15 = CalculateSubtitleCount(15, searchKey);
			ViewBag.subtitle_16 = CalculateSubtitleCount(16, searchKey);

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
            return View(viewModel);
		}

		//Search的方法
		private List<WorkViewModel> GetWorkList(string searchKey, int? subtitle, int page, int pageSize)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			List<WorkViewModel> workList;

			if (_context.Contents.Any(c => c.Title.Contains(searchKey)))
			{
				workList = vickyWorkContent.GetSearchContents(searchKey, page, pageSize);
			}
			else if (_context.Users.Any(c => c.UserName.Contains(searchKey)))
			{
				workList = vickyWorkContent.GetCreatorContents(searchKey);
			}
			else
			{
				workList = vickyWorkContent.GetSearchCommission(searchKey);
			}

			if (string.IsNullOrEmpty(searchKey) || workList.Count == 0)
			{
				searchKey = string.Empty;
				workList = vickyWorkContent.GetSearchContents("", page, pageSize);
			}

			return workList;
		}

		//計算中標籤總數的方法
		private int CalculateSubtitleCount(int subtitleId, string searchKey)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			var workList = vickyWorkContent.GetSubtitleAscending(subtitleId);
			var subtitleAsCommission = _context.Commissions.Any(c => c.Title!.Contains(searchKey));

			if (!string.IsNullOrEmpty(searchKey))
			{
				workList = workList.Where(w => w.UserName!.Contains(searchKey!) || w.Title!.Contains(searchKey!)).ToList();
				if (subtitleAsCommission)
				{
					workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey);
				}
			}

			return workList.Count;
		}

		[HttpPost]
		[Route("Vicky/GetSubtitle/{subtitleId}/{searchKey}")]
		public IActionResult GetSubtitle(int subtitleId, string searchKey, int page = 1, int pageSize = 3)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			var workList = vickyWorkContent.GetSubtitleAscending(subtitleId);
			var subtitleAsCommission = _context.Commissions.Any(c => c.Title!.Contains(searchKey));


			if (searchKey != null)
			{
				workList = workList.Where(w => w.UserName!.Contains(searchKey!) || w.Title!.Contains(searchKey!)).ToList();
				if (subtitleAsCommission)
				{
					workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey);
				}
			}

			var totalItems = workList.Count;
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			if (searchKey == null || workList.Count == 0)
			{
				searchKey = " ";
				vickyWorkContent = new VickyWorkContent(_context);
				workList = vickyWorkContent.GetSubtitleAscending(subtitleId);
				totalItems = workList.Count();
				totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
			}
			if (totalPages == 0)
			{
				totalPages = 1;
			}

			int skip = (page - 1) * pageSize;
			var pagedWorkList = workList.Skip(skip).Take(pageSize).ToList();

			var viewModel = new WorkListViewModel
			{
				WorkList = pagedWorkList,
				PageNumber = page,
				TotalPages = totalPages,
				PageSize = pageSize,

			};

			return Json(viewModel);
		}

		[HttpPost]
		[Route("Vicky/WorkOn/{Option}")]
		public IActionResult WorkOn(string Option, int page = 1, int pageSize = 3)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			List<WorkViewModel>? workList;
			if (Option == "descending")
			{
				workList = vickyWorkContent.GetContentsDescending(page, pageSize);
			}
			else
			{
				workList = vickyWorkContent.GetContentsAscending(page, pageSize);
			}
			var totalItems = _context.Contents.Count();
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			var viewModelB = new WorkListViewModel
			{
				WorkList = workList,
				PageNumber = page,
				TotalPages = totalPages,
				PageSize = pageSize
			};
			return Json(viewModelB);
		}
		[HttpPost]
		[Route("Vicky/CommissionOn/{Option}")]
		public IActionResult CommissionOn(string Option, int page = 1, int pageSize = 3)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			List<WorkViewModel>? workList;
			if (Option == "descending")
			{
				workList = vickyWorkContent.GetCommissionDescending(page, pageSize);
			}
			else
			{
				workList = vickyWorkContent.GetCommissionAscending(page, pageSize);
			}
			var totalItems = _context.Contents.Count();
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			var viewModel = new WorkListViewModel
			{
				WorkList = workList,
				PageNumber = page,
				TotalPages = totalPages,
				PageSize = pageSize
			};
			return Json(viewModel);
		}


	}
}
