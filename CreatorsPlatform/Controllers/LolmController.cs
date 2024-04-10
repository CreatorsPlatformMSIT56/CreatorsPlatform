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

        // 新增存 id 的變數，並想把他丟到 View
        public async Task<IActionResult> EventContent(int? id)
        {
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
            return View(TheEventData);
        }

        public IActionResult CreateEvent()
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

        // 活動創建(上傳Event內容至資料庫)
        [HttpPost]
        [Route("Lolm/Create")]
        public IActionResult Create(Event EventModelData)
        {
            if (MembersOnline())
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson!)!;
                var NowCreatorId = _context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;

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
                return RedirectToAction("Login", "yhu");
            }
        }

        // 活動創建(上傳範例圖片至資料庫)
        [HttpPost]
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

        // 新增投稿(上傳活動參加者的投稿 到 資料庫的EventImg表)
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

        // 獲得投稿內容
        [HttpGet]
        public EventsAndImage? EventPostContent(string EventPostId)
        {
            int EventPostIntId = Convert.ToInt32(EventPostId);
            var TheEventPostSQL = _context.EventsAndImages;
            var TheEventPost = TheEventPostSQL.FirstOrDefault(model => model.EventImageId == EventPostIntId);
            return TheEventPost;
        }

        // 刷新Like數
        [HttpPost]
        public IActionResult PostLikeChange(int LikeChange, int TheCheckedPostId)
        {
            var ThePostModel = _context.EventImages.FirstOrDefault(m => m.EventImageId == TheCheckedPostId);
            ThePostModel!.EvePostLike = LikeChange;
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
        public List<EventsAndImage> PostOrderBy(string OrderByWhat, int id)
        {
            var PostResult = (from o in _context.EventsAndImages
                              where o.EventId == id
                              select o);
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
            return AfterOrderBy;
        }

        // 編輯活動內容頁面
        public IActionResult EventEdit(int? id)
        {
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
            if (FindEventResult == null)
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
            //FindEventResult.FindAll(a => a.EventImages.ImageSample == true).ToList();
            return View();
        }

        // 編輯活動內容put:Event表
        [HttpPut]
        public string UpdateEvent(Event EventUpdateData)
        {
            var result = _context.Events.First(e => e.EventId == EventUpdateData.EventId);
            result.EventName = EventUpdateData.EventName;
            result.StartDate = EventUpdateData.StartDate;
            result.EndDate = EventUpdateData.EndDate;
            result.Description = EventUpdateData.Description;
            result.EventStyle = EventUpdateData.EventStyle;
            result.Banner = EventUpdateData.Banner;
            result.DescriptionString = EventUpdateData.DescriptionString;
            _context.Update(result);
            _context.SaveChanges();
            return "活動內容變更完成";
        }

        // 編輯活動內容post:EventImage表
        [HttpPost]
        public string UpdateEventImg(string EventImgArray, int Id)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson!)!;
            var NowCreatorId = _context.Users.Where(model => model.UserId == member.id && model.CreatorId != null).FirstOrDefault()!.CreatorId;

            string[] TheArray = JsonConvert.DeserializeObject<string[]>(EventImgArray);
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
            return "活動圖片內容變更完成";
        }
    }
}