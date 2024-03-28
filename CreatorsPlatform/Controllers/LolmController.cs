using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using static CreatorsPlatform.Controllers.HomeController;


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
            ViewBag.Eid = id;

            // ▼不知道@model的型態所以擱置
            //var result = await (from e in _context.Events
            //					join ei in _context.EventImages on e.EventId equals ei.EventId into eGroup
            //					where e.EventId == id
            //					select new { Event = e, EventImages = eGroup }).ToListAsync();
            //return View(result);

            var TheEventContext = _context.EventsAndImages;
            var TheEventData = await (from o in TheEventContext
                                      where o.EventId == id
                                      select o).ToListAsync();
            return View(TheEventData);
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
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson!)!;
            var NowCreatorId = _context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;
            if (NowCreatorId == null)
            {
                return BadRequest(); // 返回錯誤狀態碼 400;
            }
            else
            {
                Event NewEvent = new Event
                {
                    EventName = EventModelData.EventName,
                    Description = EventModelData.Description,
                    StartDate = EventModelData.StartDate,
                    EndDate = EventModelData.EndDate,
                    EventStyle = EventModelData.EventStyle,
                    Banner = EventModelData.Banner,
                    CreatorId = (int)NowCreatorId,
                    CategoryId = EventModelData.CategoryId,
                    DescriptionString = EventModelData.DescriptionString,
                    EventCancel = false
                };

                if (ModelState.IsValid != null)
                {
                    _context.Add(NewEvent);
                    _context.SaveChanges();
                    // 將這個新增活動的id傳出來給範例圖片的ajax使用
                    TempData["TheNewEventID"] = NewEvent.EventId;
                    Console.WriteLine(NewEvent.EventId);

                }
                return Ok(); // 返回成功狀態碼 200
            }
        }

        // 上傳範例圖片至資料庫
        [HttpPost]
        //public string CreateEventExImg([FromBody]JsonElement ExImgDataURLs)
        public string CreateEventExImg(EventImage NewEventImageData)
        {
            int newEventId = Convert.ToInt32(TempData["TheNewEventID"]);

            EventImage NewEventImage = new EventImage()
            {
                ImageUrl = NewEventImageData.ImageUrl,
                EventId = newEventId, // 使用从 TempData 中获取的 ID
                ImageSample = true,
                CreatorId = 1
            };
            _context.EventImages.Add(NewEventImage);
            _context.SaveChanges();
            return "OK";
        }

        // 上傳活動參加者的投稿 到 資料庫的EventImg表
        [HttpPost]
        public IActionResult CreateEventPost(EventImage NewEventImageData, int eventId)
        {
            int PostEventId = NewEventImageData.EventId;
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson!)!;
            var NowCreatorId = _context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;
            if (NowCreatorId == null)
            {
                return NotFound();
            }
            else
            {
                EventImage NewEventImage = new EventImage()
                {
                    ImageUrl = NewEventImageData.ImageUrl,
                    EventId = PostEventId,
                    ImageSample = NewEventImageData.ImageSample,
                    CreatorId = (int)NowCreatorId,
                    Description = NewEventImageData.Description,
                    ImageTitle = NewEventImageData.ImageTitle,
                    EveImgCancel = false
                };
                _context.Add(NewEventImage);
                _context.SaveChanges();
                return Ok();
            }
        }

        [HttpGet]
        public EventsAndImage? EventPostContent(string EventAndImgId)
        {
            int EventAndImgIntId = Convert.ToInt32(EventAndImgId);
            var TheEventPostSQL = _context.EventsAndImages;
            var TheEventPost = TheEventPostSQL.FirstOrDefault(model => model.EvtImgId == EventAndImgIntId);
            return TheEventPost;
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