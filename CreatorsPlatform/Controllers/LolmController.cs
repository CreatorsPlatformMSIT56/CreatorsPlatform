using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

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


		// 取得input的值並上傳到資料庫
		//[HttpPost]
		//public async Task<IActionResult> Upload([FromBody]Event model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		// 從 ViewModel 中獲取表單數據
		//		//var eventName = model.EventName;
		//		//var startDate = model.StartDate;
		//		//var endDate = model.EndDate;

		//		// 先不要處理圖片
		//		//var banner = model.Banner;

		//		// 先不要處理描述
		//		// var description = model.Description;

		//		//var eventStyle = model.EventStyle;

		//		// 創建 Event 對象並賦值
		//		//var NewEvent = new Event
		//		//{
		//		//	EventName = eventName,
		//		//	StartDate = startDate,
		//		//	EndDate = endDate,
		//			// Banner = banner,
		//			// Description = description,
		//		//	EventStyle = eventStyle
		//		//};

		//		try
		//		{
		//			// 將對象添加到資料庫並保存更改
		//			_context.Events.Add(model);
		//			await _context.SaveChangesAsync();
		//			return Ok();
		//			//return RedirectToAction(nameof(EventContent));
		//		}
		//		catch (Exception ex)
		//		{
		//			// 處理例外情況
		//			ModelState.AddModelError("", $"無法保存圖片：{ex.Message}");
		//		}
		//	}
		//	// 如果模型狀態無效，返回表單頁面
		//	return View(model);
		//}

		// 品旭的
		[HttpPost]
		public async Task<IActionResult> Upload([Bind("EventImageId,ImageUrl,EventId,ImageSample,CreatorId,Description,ImageTitle")] EventImage eventImage, int? id)
		{
			foreach (var file in HttpContext.Request.Form.Files)
			{
				if (file.Length > 0)
				{
					//var name = HttpContext.Request.Form["name"];
					//var description = HttpContext.Request.Form["description"];

					using (var ms = new MemoryStream())
					{
						await file.CopyToAsync(ms);
						var imageData = Convert.ToBase64String(ms.ToArray());
						var dataURL = $"data:{file.ContentType};base64,{imageData}";
						//var dataEvt = (int)id!;
						//int nameInInt = Convert.ToInt32(name);
						//int descriptionInInt = Convert.ToInt32(description);

						// 將DataURL保存到資料庫
						var photo = new EventImage
						{
							ImageUrl = dataURL,
							CreatorId = 7,
							EventId = 3
						};
						try
						{
							_context.EventImages.Add(photo);
							await _context.SaveChangesAsync();
							return RedirectToAction(nameof(EventContent));
						}
						catch (Exception)
						{
							_context.EventImages.Add(photo);
							await _context.SaveChangesAsync();
						}
					}
				}
			}
			return RedirectToAction(nameof(EventContent));
		}

		[HttpPost]
		[Route("Lolm/Create")]
		public IActionResult Create([FromBody] JsonElement json)
		{
			Event @event = new Event
			{
				EventName = json.GetProperty("EventName").GetString(),
				StartDate = json.GetProperty("StartDate").GetDateTime(),
				EndDate = json.GetProperty("EndDate").GetDateTime(),
				Description = json.GetProperty("Description").GetString(),
				CategoryId = json.GetProperty("CategoryID").GetInt32()
			};
			if (ModelState.IsValid != null)
			{
				_context.Add(@event);
				_context.SaveChanges();
				return Ok(); // 返回成功狀態碼 200
			}

			return BadRequest(); // 返回錯誤狀態碼 400
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