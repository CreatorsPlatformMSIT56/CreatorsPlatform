using Microsoft.AspNetCore.Mvc;
using CreatorsPlatform.CsMod.Vicky;
using CreatorsPlatform.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public string MembersIcon(int x)
        {
            string Avatar;
            var MembersIcon = (from UserData in _context.Users
                               where UserData.UserId == x
                               select UserData.Avatar).FirstOrDefault();
            if (MembersIcon != null)
            {
                Avatar = Convert.ToBase64String(MembersIcon);
            }
            else
            {
                Avatar = null;
            }

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

		[HttpGet]
        public IActionResult Search(string searchKey, int? subtitle, int? id, string sortOrder, int page = 1, int pageSize = 9)
        {
            //var workList = GetWorkList(searchKey, sortOrder,subtitle, page, pageSize);
            var workList = GetWorkList(searchKey, sortOrder, subtitle, id);

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
            ViewBag.tagId = subtitle;
            ViewBag.subtitle = id;
            ViewBag.SortOrder = sortOrder;

            Dictionary<string, SubtitleCount> subtitleCounts = [];

            for (int i = 1; i <= 16; i++)
            {
                subtitleCounts[$"subtitle_{i}"] = new SubtitleCount { Count = CalculateSubtitleCount(i, searchKey, sortOrder) };
            }

            ViewBag.SubtitleCounts = subtitleCounts;
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


			return View(viewModel);
        }

        //Search的方法
        private List<WorkViewModel> GetWorkList(string searchKey, string sortOrder, int? subtitle, int? id)
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            List<WorkViewModel> workList;
            if (_context.Contents.Any(c => c.Title.Contains(searchKey)))
            {
                workList = vickyWorkContent.GetSearchContents(searchKey, sortOrder);
            }
            else if (_context.Users.Any(c => c.UserName.Contains(searchKey)))
            {
                workList = vickyWorkContent.GetCreatorContents(searchKey);
            }
            else
            {
                workList = vickyWorkContent.GetSearchCommission(searchKey, sortOrder);
            }

            if (string.IsNullOrEmpty(searchKey) || workList.Count == 0)
            {
                searchKey = "";
                workList = vickyWorkContent.GetSearchContents("", sortOrder);
            }

            return workList;
        }

        //計算中標籤總數的方法
        private int CalculateSubtitleCount(int subtitleId, string searchKey, string sortOrder = "ascending", string buttonClicked = "")
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            List<WorkViewModel> workList = new List<WorkViewModel>();
            var subtitleAsCommission = _context.Commissions.Any(c => c.Title!.Contains(searchKey));
            if (searchKey != null)
            {
                workList = vickyWorkContent.GetSubtitleAscending(subtitleId, searchKey, sortOrder);
                workList = workList.Where(w => w.UserName!.Contains(searchKey!) || w.Title!.Contains(searchKey!)).ToList();
                if (subtitleAsCommission)
                {
                    workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey, sortOrder);
                }
                if (buttonClicked == "CommissionOn")
                {
                    workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey!, sortOrder);
                }
                if (buttonClicked == "WorkOn")
                {
                    workList = vickyWorkContent.GetSubtitleAscending(subtitleId, searchKey, sortOrder);
                }
            }
            else
            {
                workList = vickyWorkContent.GetSubtitleAscending(subtitleId, searchKey!, sortOrder);
            }
            return workList.Count;
        }

        [HttpPost]
        [Route("Vicky/GetSubtitle/{subtitleId}/{searchKey}")]
        public IActionResult GetSubtitle(int subtitleId, string searchKey, int page = 1, int pageSize = 9, string sortOrder = "", string buttonClicked = "")
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            var workList = vickyWorkContent.GetSubtitleAscending(subtitleId, searchKey, sortOrder);
            var subtitleAsCommission = _context.Commissions.Any(c => c.Title!.Contains(searchKey));
            if (searchKey != null)
            {
                workList = workList.Where(w => w.UserName!.Contains(searchKey!) || w.Title!.Contains(searchKey!)).ToList();
                if (subtitleAsCommission)
                {
                    workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey, sortOrder);
                }
            }

            var totalItems = workList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (searchKey == null || workList.Count == 0)
            {
                searchKey = "";
                vickyWorkContent = new VickyWorkContent(_context);
                if (buttonClicked == "CommissionOn")
                {
                    workList = vickyWorkContent.GetSubtitleAsCommission(subtitleId, searchKey, sortOrder);

                }
                if (buttonClicked == "WorkOn")
                {
                    workList = vickyWorkContent.GetSubtitleAscending(subtitleId, searchKey, sortOrder);
                }
                totalItems = workList.Count;
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
        [Route("Vicky/WorkOn/{sortOrder}")]
        public IActionResult WorkOn(string sortOrder, int page = 1, int pageSize = 9, string buttonClicked = "WorkOn")
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            List<WorkViewModel>? workList;
            Dictionary<string, SubtitleCount> subtitleCounts = [];
            workList = vickyWorkContent.GetSearchContents("", sortOrder);

            for (int i = 1; i <= 16; i++)
            {
                subtitleCounts[$"{i}"] = new SubtitleCount { Count = CalculateSubtitleCount(i, "", sortOrder, buttonClicked) };
            }

            int skip = (page - 1) * pageSize;
            var pagedWorkList = workList.Skip(skip).Take(pageSize).ToList();
            var totalItems = workList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var viewModel = new WorkListViewModel
            {
                WorkList = pagedWorkList,
                PageNumber = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                Count = subtitleCounts,
            };
            return Json(viewModel);
        }

        [HttpPost]
        [Route("Vicky/CommissionOn/{sortOrder}")]
        public IActionResult CommissionOn(string sortOrder, int page = 1, int pageSize = 9, string buttonClicked = "CommissionOn")
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            List<WorkViewModel>? workList;
            Dictionary<string, SubtitleCount> subtitleCounts = [];
            workList = vickyWorkContent.GetSearchCommission("", sortOrder);
            for (int i = 1; i <= 16; i++)
            {
                subtitleCounts[$"{i}"] = new SubtitleCount { Count = CalculateSubtitleCount(i, "", sortOrder, buttonClicked) };
            }

            int skip = (page - 1) * pageSize;
            var pagedWorkList = workList.Skip(skip).Take(pageSize).ToList();
            var totalItems = workList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var viewModel = new WorkListViewModel
            {
                WorkList = pagedWorkList,
                PageNumber = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                Count = subtitleCounts,
            };
            return Json(viewModel);
        }


        [HttpGet]
        [Route("Vicky/GetTags/{tagId}")]
        public IActionResult GetTags(int tagId, int page = 1, int pageSize = 9)
        {
            var vickyWorkContent = new VickyWorkContent(_context);
            var workList = vickyWorkContent.GetTagContents(tagId);
            var totalItems = workList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
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
    }
}
