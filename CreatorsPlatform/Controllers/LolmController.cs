using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers
{
	public class LolmController : Controller
	{
		private readonly ImaginkContext _context;

		public LolmController(ImaginkContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}


		//public IActionResult EventContent()
		//{
		//    return View();
		//}

		// 新增存 id 的變數，並想把他丟到 View
		public async Task<IActionResult> EventContent(int? id)
		{
			TempData["Eid"] = id;
			var imaginkContext = _context.EventsAndImages;

			// 原本想在 controller 寫 LINQ，但不會傳到 view
			//IEnumerable<EventsAndImage> query = from CCC in imaginkContext
			//                                    where CCC.EventId == id
			//                                    select CCC;

			// 取得TempData裡面裝的字串
			ViewBag.QuillContent = TempData.Peek("DataFromClient");			
			return View(await imaginkContext.ToListAsync());
		}

		public IActionResult CreateEvent()
		{
			return View();
		}

		[HttpPost]
		public string CreateEvent(string DataFromClient)
		{
			return DataFromClient;
		}

		// 取得input的值並上傳到資料庫
		[HttpPost]
		public async Task<IActionResult> Upload(Event model)
		{
			if (ModelState.IsValid)
			{
				// 從 ViewModel 中獲取表單數據
				var eventName = model.EventName;
				var startDate = model.StartDate;
				var endDate = model.EndDate;

				// 先不要處理圖片
				//var banner = model.Banner;

				// 先不要處理描述
				// var description = model.Description;
				
				var eventStyle = model.EventStyle;

				// 創建 Event 對象並賦值
				var NewEvent = new Event
				{
					EventName = eventName,
					StartDate = startDate,
					EndDate = endDate,
					// Banner = banner,
					// Description = description,
					EventStyle = eventStyle
				};

				try
				{
					// 將對象添加到資料庫並保存更改
					_context.Events.Add(NewEvent);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(EventContent));
				}
				catch (Exception ex)
				{
					// 處理例外情況
					ModelState.AddModelError("", $"無法保存圖片：{ex.Message}");
				}
			}

			// 如果模型狀態無效，返回表單頁面
			return View(model);
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var eventsAndImages = await _context.EventsAndImages
				.FirstOrDefaultAsync(m => m.EventId == 1);
			if (eventsAndImages == null)
			{
				return NotFound();
			}

			return View(eventsAndImages);
		}
	}
}