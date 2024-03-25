using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
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

			id = 1;
			Event TheEvent = _context.Events.First( i => i.EventId == id);
			EventImage TheEventImg = _context.EventImages.First( i => i.EventId == id);

			return View(await imaginkContext.ToListAsync());
		}

		public IActionResult CreateEvent()
		{

			return View();
		}

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
							return RedirectToAction("EventContent");
						}
						catch (Exception)
						{
							_context.EventImages.Add(photo);
							await _context.SaveChangesAsync();
						}
					}
				}
			}
			return RedirectToAction("EventContent");
		}

		// 上傳Event內容至資料庫
		[HttpPost]
		[Route("Lolm/Create")]
		public IActionResult Create(Event EventModelData)
		{
			Event NewEvent = new Event
			{
				EventName = EventModelData.EventName,
				Description = EventModelData.Description,
				StartDate = EventModelData.StartDate,
				EndDate = EventModelData.EndDate,
				EventStyle = EventModelData.EventStyle,
				Banner = EventModelData.Banner,
				CategoryId = EventModelData.CategoryId
			};			

			if (ModelState.IsValid != null)
			{
				_context.Add(NewEvent);

				// 將這個新增活動的id傳出來給範例圖片的ajax使用
				TempData["TheNewEventID"] = NewEvent.EventId;
				_context.SaveChanges();
				return Ok(); // 返回成功狀態碼 200
			}

			return BadRequest(); // 返回錯誤狀態碼 400
		}

		// 上傳範例圖片至資料庫
		[HttpPost]
		//public string CreateEventExImg([FromBody]JsonElement ExImgDataURLs)
		public string CreateEventExImg(EventImage NewEventImageData)
		{
			EventImage NewEventImage = new EventImage()
			{
				ImageUrl = NewEventImageData.ImageUrl,
				
				EventId = 1,
				ImageSample = true,
				CreatorId = 1
			};
			_context.EventImages.Add(NewEventImage);
			_context.SaveChanges();
			return "OK";
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