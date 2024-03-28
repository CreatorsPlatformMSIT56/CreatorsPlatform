using Microsoft.AspNetCore.Mvc;
using CreatorsPlatform.CsMod.Vicky;
using CreatorsPlatform.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.CodeAnalysis.Options;


namespace CreatorsPlatform.Controllers
{
	public class VickyController : Controller
	{
		private readonly ImaginkContext _context;

		public VickyController(ImaginkContext context)
		{
			_context = context;
		}


		[HttpGet]
		public IActionResult Search(int page = 1, int pageSize = 3)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			var workList = vickyWorkContent.GetContentsAscending(page, pageSize);
			var totalItems = _context.Contents.Count(); // 获取总条目数
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize); // 计算总页数
																			   //var totalPages = totalItems; // 计算总页数

			var viewModel = new WorkListViewModel
			{
				WorkList = workList,
				PageNumber = page,
				TotalPages = totalPages,
				PageSize = pageSize
			};

			return View(viewModel);
		}


		[HttpPost]
		[Route("Vicky/GetSubtitle/{subtitleId}")]
		public IActionResult GetSubtitle(int subtitleId, int page = 1, int pageSize = 3)
		{
			var vickyWorkContent = new VickyWorkContent(_context);
			var workList = vickyWorkContent.GetSubtitleAscending(subtitleId);
			var totalItems = workList.Count;  // 根據實際請求返回的項目數計算總項目數
			var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

			int skip = (page - 1) * pageSize;

			// 使用 Skip() 和 Take() 方法对 workList 进行分页
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
		public IActionResult WorkOn(string Option, int page =1, int pageSize = 3)
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
		public IActionResult CommissionOn(CreatorServiceContent commission)
		{
			List<CreatorService> commissionList = commission.GetCommissionList();
			return Json(commissionList);
		}

	}
}
