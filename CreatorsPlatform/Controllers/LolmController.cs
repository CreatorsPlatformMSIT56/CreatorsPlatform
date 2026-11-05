using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using static CreatorsPlatform.Controllers.yhuController;


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
        //臨時增加
        public string? MembersIcon(int x)
        {
            string? Avatar;
            var MembersIcon = (from UserData in _context.Users
                               where UserData.UserId == x
                               select UserData.Avatar).FirstOrDefault();
            if (MembersIcon != null)
            {
                Avatar = $"/images/avatar/{x}";
            }
            else
            {
                Avatar = null;
            }

            return Avatar;
        }
        public bool MembersOnline() => SessionMember.Get(HttpContext, _context) != null;
        //

        // 新增存 id 的變數，並想把他丟到 View
        public async Task<IActionResult> EventContent(int? id)
        {
            if (id == null || !_context.Events.Any(e => e.EventId == id)) return NotFound();
            ViewBag.Eid = id;
            string ThePageStyle;
            // 把style丟到前面
            if (_context.Events.FirstOrDefault(m => m.EventId == id)!.EventStyle != null)
            {
                ThePageStyle = _context.Events.FirstOrDefault(m => m.EventId == id)!.EventStyle!;
                string[] JsonArray = JsonConvert.DeserializeObject<string[]>(ThePageStyle)!;
                ViewBag.TitleStyle = JsonArray[0];
                ViewBag.IntroStyle = JsonArray[1];
            };
            var TheEventContext = _context.EventsAndImages;
            var TheEventData = await (from o in TheEventContext
                                      where o.EventId == id
                                      select o).OrderByDescending(n => n.EvtImgId).ToListAsync();
            //
            if (MembersOnline())
            {
                var member = SessionMember.Get(HttpContext, _context);
                if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
            };
            //
            return View(TheEventData);
        }

        public IActionResult CreateEvent()
        {
            if (!MembersOnline()) return RedirectToAction("Login", "yhu");
            //
            if (MembersOnline())
            {
                var member = SessionMember.Get(HttpContext, _context);
                if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
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

        // 品旭的
        [HttpPost]
        public async Task<IActionResult> Upload([Bind("EventImageId,ImageUrl,EventId,ImageSample,CreatorId,Description,ImageTitle")] EventImage eventImage, int? id)
        {
            if (!MembersOnline()) return Unauthorized();
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

        // 活動創建(上傳Event內容至資料庫)
        [HttpPost]
        [Route("Lolm/Create")]
        public IActionResult Create(Event EventModelData)
        {
            if (MembersOnline())
            {
                var member = SessionMember.Get(HttpContext, _context);
                if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
                var NowCreatorId = _context.Users.Where(user => user.UserId == member.id).Select(user => user.CreatorId).FirstOrDefault();
                if (NowCreatorId == null) return StatusCode(StatusCodes.Status403Forbidden);

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
                    _context.Add(NewEvent);
                    _context.SaveChanges();
                    // 將這個新增活動的id傳出來給範例圖片的ajax使用
                    TempData["TheNewEventID"] = NewEvent.EventId;
                    Console.WriteLine(NewEvent.EventId);
                    return RedirectToAction("Index", "HotGuy");

            }
            else
            {
                return Unauthorized();
            }
        }

        // 活動創建(上傳範例圖片至資料庫)
        [HttpPost]
        public IActionResult CreateEventExImg(EventImage NewEventImageData)
        {
            var member = SessionMember.Get(HttpContext, _context);
            if (member == null) return Unauthorized();
            var creatorId = _context.Users.Where(user => user.UserId == member.id).Select(user => user.CreatorId).FirstOrDefault();
            if (creatorId == null) return StatusCode(StatusCodes.Status403Forbidden);
            int newEventId = Convert.ToInt32(TempData["TheNewEventID"]);

            EventImage NewEventImage = new EventImage()
            {
                ImageUrl = NewEventImageData.ImageUrl,
                EventId = newEventId, // 使用从 TempData 中获取的 ID
                ImageSample = true,
                CreatorId = creatorId.Value
            };
            _context.EventImages.Add(NewEventImage);
            _context.SaveChanges();
            return Content("OK");
        }

        // 新增投稿(上傳活動參加者的投稿 到 資料庫的EventImg表)
        [HttpPost]
        public IActionResult CreateEventPost(EventImage NewEventImageData, int eventId)
        {
            int PostEventId = NewEventImageData.EventId;
            var member = SessionMember.Get(HttpContext, _context);
            if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
            var NowCreatorId = _context.Users.Where(user => user.UserId == member.id).Select(user => user.CreatorId).FirstOrDefault();
            if (NowCreatorId == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
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

        // 獲得投稿內容
        [HttpGet]
        public IActionResult EventPostContent(string EventPostId)
        {
            int EventPostIntId = Convert.ToInt32(EventPostId);
            var TheEventPostSQL = _context.EventsAndImages;
            var TheEventPost = TheEventPostSQL.Where(model => model.EventImageId == EventPostIntId)
                .Select(model => new { model.EventImageId, model.ImgTitle, model.ImgDes, model.ImgCreator, model.ImgCreName, model.EvePostLike }).FirstOrDefault();
            return Json(TheEventPost);
        }

        // 刷新Like數
        [HttpPost]
        public IActionResult PostLikeChange(int LikeChange, int TheCheckedPostId)
        {
            if (!MembersOnline()) return Unauthorized();
            var ThePostModel = _context.EventImages.FirstOrDefault(m => m.EventImageId == TheCheckedPostId);
            if (ThePostModel == null) return NotFound();
            ThePostModel.EvePostLike = LikeChange;
            _context.Update(ThePostModel);
            _context.SaveChanges();
            return Ok();
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

        // 更新活動中的投稿排序
        [HttpGet]
        public IActionResult PostOrderBy(string OrderByWhat, int id)
        {
            var PostResult = (from o in _context.EventsAndImages
                              where o.EventId == id
                              select new EventsAndImage
                              {
                                  EventImageId = o.EventImageId,
                                  ImageSample = o.ImageSample,
                                  ImgTitle = o.ImgTitle,
                                  EvePostLike = o.EvePostLike,
                                  ImgCreator = o.ImgCreator,
                                  ImgCreName = o.ImgCreName
                              });
            List<EventsAndImage> AfterOrderBy = new List<EventsAndImage>();
            switch (OrderByWhat)
            {
                case "HighLike":
                    AfterOrderBy = PostResult.OrderByDescending(n => n.EvePostLike).ToList();
                    break;
                case "LowLike":
                    AfterOrderBy = PostResult.OrderBy(n => n.EvePostLike).ToList();
                    break;
                case "NewPost":
                    AfterOrderBy = PostResult.OrderByDescending(n => n.EventImageId).ToList();
                    break;
                case "OldPost":
                    AfterOrderBy = PostResult.OrderBy(n => n.EventImageId).ToList();
                    break;
                default:
                    // 处理未知情况
                    break;
            }
            return Json(AfterOrderBy.Select(item => new { item.EventImageId, item.ImageSample, item.ImgTitle, item.EvePostLike, item.ImgCreator, item.ImgCreName }));
        }

        // 編輯活動內容頁面
        public IActionResult EventEdit(int? id)
        {
            if (!MembersOnline()) return RedirectToAction("Login", "yhu");
            if (id == null)
            {
                return NotFound();
            }
            // 找對應id的活動
            var FindEventResult = (from e in _context.Events
                                   where e.EventId == id
                                   select new
                                   {
                                       TheEventId = e.EventId,
                                       EventName = e.EventName,
                                       Description = e.Description,
                                       StartDate = e.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                       EndDate = e.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                       Style = e.EventStyle,
                                       EventTitleColor = e.EventStyle != null ? e.EventStyle.Substring(2, 7) : null,
                                       EventIntroColor = e.EventStyle != null ? e.EventStyle.Substring(12, 7) : null,
                                       Banner = e.Banner
                                   }).ToList();
            if (FindEventResult.Count == 0)
            {
                return NotFound();
            }
            ViewBag.Event = FindEventResult;
            // 找對應id的範例圖片
            var FindEventSampleImg = (from i in _context.EventImages
                                      where i.EventId == id && i.ImageSample == true && i.EveImgCancel == false
                                      select i).ToList();
            if (FindEventSampleImg != null)
            {
                ViewBag.SampleImg = FindEventSampleImg;
            }
            //
            if (MembersOnline())
            {
                var member = SessionMember.Get(HttpContext, _context);
                if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
            };
            //
            //FindEventResult.FindAll(a => a.EventImages.ImageSample == true).ToList();
            return View();
        }

        // 編輯活動內容put:Event表
        [HttpPut]
        public IActionResult UpdateEvent(Event EventUpdateData)
        {
            if (!MembersOnline()) return Unauthorized();
            var result = _context.Events.FirstOrDefault(e => e.EventId == EventUpdateData.EventId);
            if (result == null) return NotFound();
            result.EventName = EventUpdateData.EventName;
            result.StartDate = EventUpdateData.StartDate;
            result.EndDate = EventUpdateData.EndDate;
            result.Description = EventUpdateData.Description;
            result.EventStyle = EventUpdateData.EventStyle;
            result.Banner = EventUpdateData.Banner;
            result.DescriptionString = EventUpdateData.DescriptionString;
            _context.Update(result);
            _context.SaveChanges();
            return Content("活動內容變更完成");
        }

        // 編輯活動內容post:EventImage表
        [HttpPost]
        public IActionResult UpdateEventImg(string EventImgArray, int Id)
        {
            var member = SessionMember.Get(HttpContext, _context);
            if (member == null) return HttpMethods.IsGet(Request.Method) ? RedirectToAction("Login", "yhu") : Unauthorized();
            var NowCreatorId = _context.Users.Where(user => user.UserId == member.id).Select(user => user.CreatorId).FirstOrDefault();
                if (NowCreatorId == null) return StatusCode(StatusCodes.Status403Forbidden);

            string[]? TheArray = JsonConvert.DeserializeObject<string[]>(EventImgArray);
            if (TheArray == null) return BadRequest();
            if (_context.EventImages.Any(e => e.EventId == Id && e.ImageSample == true))
            {
                var result = _context.EventImages.Where(e => e.EventId == Id && e.ImageSample == true).ToList();
                foreach (var item in result)
                {
                    item.EveImgCancel = true;
                    _context.Update(item);
                }
            }
            for (int i = 0; i < TheArray.Length; i++)
            {
                EventImage NewEventImage = new EventImage()
                {
                    ImageUrl = TheArray[i],
                    EventId = Id, // 使用从 TempData 中获取的 ID
                    ImageSample = true,
                    CreatorId = (int)NowCreatorId,
                    EveImgCancel = false
                };
                _context.EventImages.Add(NewEventImage);
            }
            _context.SaveChanges();
            return Ok("活動圖片內容變更完成");
        }
    }
}
