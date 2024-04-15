using CreatorsPlatform.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Composition;
using System.Diagnostics.Tracing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CreatorsPlatform.Controllers
{
    public class yhuController : Controller
    {
        private readonly ImaginkContext _context;
        public yhuController(ImaginkContext context)
        {
            _context = context;
        }
        public class MemberData
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

        };

		[HttpGet]
        //系統自動發驗證碼信件
        public void sendGmail(string MEmail, string verifyCode)
        {
            var member = _context.Users.FirstOrDefault(x => x.Email == MEmail);
            var result = "";
            if (member != null)
            {
                result = member.Email;
            }
            else
            {
                result = "發送郵件失敗";
            }
            System.Diagnostics.Debug.WriteLine("顯示的" + result);
            //var result = member.Email;
            //System.Diagnostics.Debug.WriteLine("顯示的" + result);

            if (member != null)
            {
                MailMessage mail = new MailMessage();
                //                          前面是發信的email  後面是顯示的名稱   
                mail.From = new MailAddress("Aa0977706956@gmail.com", "系統驗證碼發送");
                //收件者email
                mail.To.Add(MEmail);//result
                                    //mail.To.Add("wryi636@gmail.com");//result\
                mail.To.Add("Aa0977706956@gmail.com");//result 收到信箱
                                                      //設定優先權
                mail.Priority = MailPriority.Normal;
                //標題
                mail.Subject = "IMAGINK_身分驗證，此為系統自動發信，請勿回信";
                //內容
                mail.Body = "<h1>IMAGINK系統 驗證碼</h1>\r\n" +
                            "<p>以下是您本次修改密碼的驗證碼</p>\r\n" +
                            "<h3>驗證碼:</h3>\r\n" +
                            "<h3>" + verifyCode + "</h3>\r\n";
                //內容使用html
                mail.IsBodyHtml = true;
                //設定gmail的smtp(這是google的)
                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
                //在gmail的帳號密碼
                MySmtp.Credentials = new System.Net.NetworkCredential("Aa0977706956@gmail.com", "kpegknehzepacohd");
                //開啟ssl
                MySmtp.EnableSsl = true;
                //發送郵件
                MySmtp.Send(mail);
                //放掉宣告出來的MySetp
                MySmtp = null;
                //放掉宣告出來的mail
                mail.Dispose();
                System.Diagnostics.Debug.WriteLine("顯示" + "郵件發送成功");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("顯示" + "郵件未寄送");
                ViewBag.ErrorMessage = "發送郵件失敗";
            }
        }

		[HttpGet]
		public IActionResult FTP()
		{
			ViewBag.MembersOnline = MembersOnline();

			return View();
		}

		[HttpPost]
		public IActionResult FTPGet(string userEmail, string userPassword)
		{
            ViewBag.MembersOnline = MembersOnline();

			var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);

			if (user != null)
			{
				// 更新密碼
				user.Password = userPassword;
				_context.SaveChanges(); 
			}

            return Json(user);
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
        public IActionResult PersonalUser()
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson == null)
            {
                ViewBag.MembersOnline = MembersOnline();
                return View("Login");
            }
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var UserPasswd = (from UserPasswdData in _context.Users
                              where UserPasswdData.Email == member.Email
                              select new { UserPasswdData.Password, UserPasswdData.CreatorId }).ToList();
            if (member.Password == UserPasswd[0].Password.ToString())
            {
                var NewmesgData = (from Newmesg in _context.Contents
                                   join UserData in _context.Users on Newmesg.CreatorId equals UserData.CreatorId
                                   select new
                                   {
                                       Newmesg.Title,
                                       Newmesg.Description,
                                       UserData.UserName,
                                       Newmesg.ImageUrl,
                                       Newmesg.UploadDate,
                                       Newmesg.ContentId
                                   })
                                       .OrderByDescending(item => item.UploadDate).Take(5).ToList();
                ViewBag.NewmesgData = NewmesgData;
                ViewBag.MemberCreatorId = UserPasswd[0].CreatorId;
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
                return View("PersonalUser");
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
                return View("Login");
            };
        }
        [HttpPost]
        public ActionResult PersonalUser(int _CurrentMsg, string tapy)
        {
            switch (tapy)
            {
                case "newmsg":
                    var NewmesgData = (from Newmesg in _context.Contents
                                       join UserData in _context.Users on Newmesg.CreatorId equals UserData.CreatorId
                                       select new { Newmesg.ContentId, Newmesg.Title, Newmesg.Description, UserData.UserName, Newmesg.ImageUrl, Newmesg.UploadDate })
                                       .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5).ToList();
                    return Json(NewmesgData.ToList());
                case "subscribemsg":

                    var memberJson = HttpContext.Session.GetString("key");
                    MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                    //var SubscriptionDataList = (from UsersData in _context.Users
                    //                            join SubscriptionsData in _context.Subscriptions on UsersData.UserId equals SubscriptionsData.UserId
                    //                            where SubscriptionsData.UserId == member.id
                    //                            select SubscriptionsData.PlanId);

                    //var subscribemsgData = (from Contents in _context.Contents
                    //                        join PlansData in _context.Plans on Contents.PlanId equals PlansData.PlanId
                    //                        join UsersData in _context.Users on Contents.CreatorId equals UsersData.CreatorId
                    //                        where SubscriptionDataList.Contains(Contents.PlanId)
                    //                        select new { Contents.ContentId, Contents.Title, Contents.Description, UsersData.UserName, Contents.ImageUrl, Contents.UploadDate })
                    //                   .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5).ToList();
                    //return Json(subscribemsgData);

                    // 關注功能
                    // 找出他關注的所有作者
                    var result = (from f in _context.Follows
                                  where f.UserId == member.id
                                  select f.CreatorId).ToList();
                    // 找他關注的作者的所有投稿依時間排序
                    var AllFollowCreatorPost = (from content in _context.Contents
                                                join user in _context.Users on content.CreatorId equals user.CreatorId
                                                where result.Contains(content.CreatorId)
                                                orderby content.ContentId descending
                                                select new { Content = content, UserName = user.UserName }).ToList().Take(5);

                    return Json(AllFollowCreatorPost);

                case "eventmsg":
                    var eventmsgData = (from eventmsg in _context.Events
                                        select new
                                        {
                                            eventmsg.EventId,
                                            eventmsg.EventName,
                                            eventmsg.Description,
                                            eventmsg.Banner,
                                            eventmsg.StartDate,
                                            eventmsg.EndDate
                                        }).ToList();
                    Console.WriteLine(eventmsgData[1]);
                    return Json(eventmsgData);
                default:
                    return Json("Eeeor");

            }
        }
        public IActionResult IMAGINK()
        {
            //var ContentsData =
            //                    from CreatorsData in _context.Creators
            //                    join UserData in _context.Users on CreatorsData.CreatorId equals UserData.CreatorId
            //                    select new { UserData.UserName, UserData.Avatar, CreatorsData.CreatorId, CreatorsData.Description };
            //作品喜歡數合計排序作者id
            var DefaultCreatorsData =
                             ((from DefaultContents in _context.Contents
                               group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                               orderby PopularityRranking.Sum(r => r.Likes) descending
                               select new
                               {
                                   CreatorId = PopularityRranking.Key,
                                   UserLikes = PopularityRranking.Sum(r => r.Likes)
                               }).OrderByDescending(item => item.UserLikes).Take(6));
            var xxx = DefaultCreatorsData.ToList();
            var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.CreatorId).ToList();
            //作者依喜歡數排序並取則頭像等
            var AuthorProfile = (from UsersData in _context.Users
                                 join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                 join topCreator in DefaultCreatorsData on UsersData.CreatorId equals topCreator.CreatorId
                                 where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                 orderby topCreator.UserLikes descending
                                 select new
                                 {
                                     UsersData.UserId,
                                     Avatar = UsersData.Avatar != null ? Convert.ToBase64String(UsersData.Avatar) : null,
                                     UsersData.UserName,
                                     UsersData.CreatorId,
                                     Description = Introduction.Description.Length > 26 ?
                                    Introduction.Description.Substring(0, 26) + "..." : Introduction.Description,
                                 });
			//依作者照第一個作者群找作品
			var DefaultContentsData = ((from DefaultContents in _context.Contents
                                        where DefaultContents.CreatorId == xxx[0].CreatorId
                                        select new
                                        {
                                            DefaultContents.ContentId,
                                            DefaultContents.ImageUrl,
                                            DefaultContents.Title,
                                            DefaultContents.UploadDate
                                        }).OrderByDescending(item => item.UploadDate).Take(3));
            var EventDataList = (from EventData in _context.Events
                                 select new
                                 {
                                     EventData.EventId,
                                     EventData.Banner
                                 }).OrderByDescending(x => x.EventId).Take(3);


            var DefaultContentsDatalist = DefaultContentsData.ToList();

            Console.WriteLine(DefaultContentsDatalist[1].Title);

            ViewBag.EventDataList = EventDataList.ToList();
            ViewBag.DefaultContentsData = DefaultContentsData.ToList();
            ViewBag.AuthorProfile = AuthorProfile.ToList();
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

            return View();
        }
        [HttpPost]
        public ActionResult CreatorsChange(int data)
        {
            if (data > 6)
            {
                switch (data)
                {
                    case 7:
                        var DefaultCreatorsData =
                          (from DefaultContents in _context.Contents
                           group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                           select new
                           {
                               CreatorId = PopularityRranking.Key,
                               UserLikes = PopularityRranking.Sum(r => r.Likes)
                           }).OrderByDescending(item => item.UserLikes).Take(6);
                        var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.CreatorId).ToList();
                        var AuthorProfile = (from UsersData in _context.Users
                                             join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                             join Default in DefaultCreatorsData on UsersData.CreatorId equals Default.CreatorId
                                             where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                             orderby Default.UserLikes descending
                                             select new
                                             {
                                                 UsersData.UserId,
                                                 Avatar = UsersData.Avatar != null ? Convert.ToBase64String(UsersData.Avatar) : null,
                                                 UsersData.UserName,
                                                 UsersData.CreatorId,
                                                 Description = Introduction.Description.Length > 10 ?
                                                 Introduction.Description.Substring(0, 10) + "..." : Introduction.Description
                                             }
                                   );
                        return Json(AuthorProfile.ToList());
                    case 8:
                        var NewReportData = (from UserDescription in _context.Creators
                                             join NewReport in _context.Users on UserDescription.CreatorId equals NewReport.CreatorId
                                             select new
                                             {
                                                 NewReport.UserId,
                                                 Avatar = NewReport.Avatar != null ? Convert.ToBase64String(NewReport.Avatar) : null,
                                                 NewReport.UserName,
                                                 NewReport.CategoryId,
                                                 Description = UserDescription.Description.Length > 10 ? UserDescription.Description.Substring(0, 10) + "..." : UserDescription.Description
                                             }).OrderByDescending(u => u.UserId).Take(6);
                        return Json(NewReportData.ToList());
                    default:
                        return Json(null);
                }
            }
            else
            {


                var CATCreatorsData =
                           (from DefaultContents in _context.Contents
                            where DefaultContents.CategoryId == data
                            group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                            select new
                            {
								CreatorId = PopularityRranking.Key,
                                UserLikes = PopularityRranking.Sum(r => r.Likes)
                            }).OrderByDescending(item => item.UserLikes).Take(6);

                var userIDsArray = (from Data in CATCreatorsData
                                    select Data.CreatorId).ToList();
                var CreatorsData = (from UserData in _context.Users            
                                    join Creators in _context.Creators on UserData.CreatorId equals Creators.CreatorId
                                    join CATCData in CATCreatorsData on UserData.CreatorId equals CATCData.CreatorId
                                    where userIDsArray.Contains(Creators.CreatorId)
                                    orderby CATCData.UserLikes descending
                                    select new
                                    {
                                        UserData.UserId,
                                        Avatar = UserData.Avatar!=null?Convert.ToBase64String(UserData.Avatar): null,
                                        UserData.UserName,
                                        UserData.CategoryId,
                                        Description = Creators.Description.Length > 10 ?
                                        Creators.Description.Substring(0, 10)+ "..." : Creators.Description
                                    });
                if (userIDsArray.Count == 0)
                {
                    var DefaultCreatorsData =
                            (from DefaultContents in _context.Contents
                             group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                             select new
                             {
                                 CreatorId = PopularityRranking.Key,
                                 UserLikes = PopularityRranking.Sum(r => r.Likes)
                             }).OrderByDescending(item => item.UserLikes).Take(6);
                    var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.CreatorId).ToList();
                    var AuthorProfile = (from UsersData in _context.Users
                                         join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                         join Default in DefaultCreatorsData on UsersData.CreatorId equals Default.CreatorId
                                         where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                         orderby Default.UserLikes descending
                                         select new
                                         {
                                             UsersData.UserId,
                                             Avatar = UsersData.Avatar != null ? Convert.ToBase64String(UsersData.Avatar) : null,
                                             UsersData.UserName,
                                             UsersData.CategoryId,
                                             Description = Introduction.Description.Length > 10 ?
                                             Introduction.Description.Substring(0, 10) + "..." : Introduction.Description
                                         }
                               );
                    return Json(AuthorProfile.ToList());
                }
                return Json(CreatorsData.ToList());
            }


        }
        [HttpPost]
        public ActionResult WorkChanges(int data)
        {
            var CreatorsID = (from Creators in _context.Users
                              where Creators.UserId == data
                              select Creators.CreatorId).FirstOrDefault();
            var DefaultContentsData = (from DefaultContents in _context.Contents
                                       where DefaultContents.CreatorId == CreatorsID
                                       select new
                                       {
                                           DefaultContents.ContentId,
                                           ImageUrl = Convert.ToBase64String(DefaultContents.ImageUrl),
                                           DefaultContents.Title,
                                           DefaultContents.UploadDate
                                       }).OrderByDescending(item => item.UploadDate).Take(3);
            return Json(DefaultContentsData);
        }
        public IActionResult Payment(int id)
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson != null)
            {
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                var subPayment = (from p in _context.Plans
                                  join i in _context.Users on p.CreatorId equals i.CreatorId
                                  where p.PlanId == id
                                  select new
                                  {
                                      planId = p.PlanId,
                                      planName = p.PlanName,
                                      planPrice = p.PlanPrice,
                                      planDes = p.Description,
                                      creatorName = i.UserName,
                                      creatorAvatar = i.Avatar,
                                      creatorId = i.CreatorId
                                  });

                ViewBag.subPay = subPayment.ToList();
                ViewBag.Id = member.id;
                ViewBag.PlanId = id;

                var PlanIdToCreator = (from p in _context.Plans
                                       where p.PlanId == id
                                       select new { creatorId = p.CreatorId, }).ToList();
                ViewBag.Cid = PlanIdToCreator.FirstOrDefault().creatorId;

                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
                return View();
            }
            ViewBag.MembersOnline = MembersOnline();
            return View("Login");
        }


        [HttpPost]
        public ActionResult SubPayments(Subscription subPay)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var SubFollow = (from f in _context.Follows
                             join i in _context.Users on f.UserId equals i.UserId
                             join c in _context.Creators on i.CreatorId equals c.CreatorId
                             where f.UserId == subPay.UserId && f.CreatorId == subPay.CreatorId
                             select f.FollowId).FirstOrDefault();

            if (memberJson != null)
            {
                // 如果用戶從沒follow作者過
                if (SubFollow == 0)
                {
                    Follow follow = new Follow
                    {
                        CreatorId = subPay.CreatorId,
                        UserId = subPay.UserId,
                        Unfollow = false
                    };
                    if (ModelState.IsValid != null)
                    {
                        _context.Add(follow);
                        _context.SaveChanges();

                        Console.WriteLine("杰哥不要2");
                    }
                }
                else
                {
                    // 如果用戶取關作者 把關注加回去 夠貼心吧
                    if (_context.Follows.Any(f => f. UserId == subPay.UserId && f.CreatorId == subPay.CreatorId && f.Unfollow == true))
                    {
                        var FollowChange = _context.Follows.SingleOrDefault(f => f.UserId == subPay.UserId && f.CreatorId == subPay.CreatorId && f.Unfollow == true);
                        FollowChange.Unfollow = false;
                        _context.Update(FollowChange);
                        _context.SaveChanges();
                    }

                    Console.WriteLine("已追蹤");
                }

                Console.WriteLine("我要進來囉");
                var sDate = DateTime.Now;
                DateOnly? ssDate = DateOnly.FromDateTime(sDate);
                var eDate = sDate.AddMonths(1);
                DateOnly? eeDate = DateOnly.FromDateTime(eDate);

                Subscription NewSub = new Subscription
                {
                    StartDate = ssDate,
                    EndDate = eeDate,
                    PaymentMade = true,
                    CreatorId = subPay.CreatorId,
                    PlanId = subPay.PlanId,
                    UserId = subPay.UserId
                };

                if (ModelState.IsValid != null)
                {
                    _context.Add(NewSub);
                    _context.SaveChanges();

                    Console.WriteLine("杰哥不要");
                    return Ok();
                }
                return BadRequest();

            }
            return BadRequest();
        }
        public IActionResult EntrustPayment(int id)
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson != null)
            {
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

                var ComCheck = (from c in _context.Commissions
                                join i in _context.Users on c.CreatorId equals i.CreatorId
                                where c.CommissionId == id
                                select new
                                {
                                    commissionId = c.CommissionId,
                                    creatorName = i.UserName,
                                    creatorAvatar = i.Avatar,
                                    comPriceMin = c.PriceMin,
                                    comPriceMax = c.PriceMax,
                                    commissionTitle = c.Title,
                                    comDescription = c.Description
                                });

                ViewBag.ComCheck = ComCheck.ToList();
                ViewBag.Id = member.id;
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();

                return View();
            }
            ViewBag.MembersOnline = MembersOnline();
            return View("Login");

        }
        [HttpPost]
        [Route("yhu/ComOrders")]
        public ActionResult ComOrders(CommissionOrder ComOrder)
        {
            Console.WriteLine("我要進來囉");
            var sDate = DateTime.Now;
            DateOnly ssDate = DateOnly.FromDateTime(sDate);

            CommissionOrder cOrder = new CommissionOrder
            {
                Price = ComOrder.Price,
                OrderDate = ssDate,
                Description = ComOrder.Description,
                CommissionId = ComOrder.CommissionId,
                UserId = ComOrder.UserId,
                WorkStatus = "待確認中"
            };

            if (ModelState.IsValid != null)
            {
                _context.Add(cOrder);
                _context.SaveChanges();

                Console.WriteLine("杰哥不要");
                return Ok();
            }
            return BadRequest();
        }

        public IActionResult Individual()
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson != null)
            {
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                var Avatar = (from UserData in _context.Users
                              where UserData.Email == member.Email
                              select UserData.Avatar).FirstOrDefault();
                var CreatorsCheck = (from UserData in _context.Users
                                     where UserData.UserId == member.id
                                     select UserData.CreatorId).FirstOrDefault();
               

                if (CreatorsCheck != null)
                {
                    ViewBag.CreatorsCheck = true;
                }
                else
                {
                    ViewBag.CreatorsCheck = false;
                }
                ViewBag.Email = member.Email;
                ViewBag.Name = member.Name;
                if (Avatar != null)
                {
                    ViewBag.Avatar = Convert.ToBase64String(Avatar);
                }
                ViewBag.MembersIcon = MembersIcon(member.id);
                ViewBag.MembersOnline = MembersOnline();
                return View();
            }
            else
            {
                ViewBag.MembersOnline = MembersOnline();
                return View("Login");
            }

        }
        [HttpPost]
        public ActionResult IndividualData(string type, int step)
        {
            var memberJson = HttpContext.Session.GetString("key");
            if (memberJson == null)
            {
                View("Login");
            };
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var CreatorId = (from UserData in _context.Users
                             where UserData.UserId == member.id
                             select UserData.CreatorId).FirstOrDefault();


            switch (type)
            {
                case "GeneralSettings":
                    var Avatar = (from UserData in _context.Users
                                  where UserData.Email == member.Email
                                  select UserData.Avatar).FirstOrDefault();
                    string AvatarIcon=null;
                    if (Avatar!=null)
                    {
                        AvatarIcon = Convert.ToBase64String(Avatar);
                    }

                    return Json(new { Email = member.Email, Name = member.Name, Avatar = AvatarIcon });
                case "ConsumptionRecord":
                    switch (step)
                    {
                        case 1:
                            var Subscription = (from SubscriptionData in _context.Subscriptions
                                                where SubscriptionData.UserId == member.id
                                                join PlansData in _context.Plans on SubscriptionData.PlanId equals PlansData.PlanId
                                                join UserData in _context.Users on SubscriptionData.CreatorId equals UserData.CreatorId
                                                select new
                                                {
                                                    PlansData.PlanName,
                                                    UserData.UserName,
                                                    PlansData.Description,
                                                    PlansData.PlanLevel,
                                                    PlansData.PlanPrice,
                                                    SubscriptionData.EndDate
                                                });
                            return Json(Subscription.ToList());
                        case 2:
                            var Order1 = (from OrderData in _context.CommissionOrders
                                         join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                         join UserData in _context.Users on OrderProjectCons.CreatorId equals UserData.CreatorId
                                          where OrderData.UserId == member.id && OrderData.WorkStatus== "待確認中"
                                          select new
                                         {
                                             OrderData.CommissionId,
                                              OrderData.CommissionOrderId,
                                              UserData.UserName,
                                              OrderProjectCons.Title,
                                             OrderData.Price,
                                             OrderData.OrderDate,
                                             OrderData.WorkStatus
                                         });
                            return Json(Order1.ToList());
                        case 3:
                            var Order2 = (from OrderData in _context.CommissionOrders
                                         join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                         join UserData in _context.Users on OrderProjectCons.CreatorId equals UserData.CreatorId
                                         where OrderData.UserId == member.id && OrderData.WorkStatus == "價格回覆"
                                          select new
                                         {
                                             OrderData.CommissionId,
                                              OrderData.CommissionOrderId,
                                              UserData.UserName,
                                             OrderProjectCons.Title,
                                              OrderData.Description,
                                             OrderData.Price,
                                             OrderData.OrderDate,
                                             OrderData.WorkStatus
                                         });
                            return Json(Order2.ToList());
                        case 4:
                            var Order3 = (from OrderData in _context.CommissionOrders
                                         join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                          join UserData in _context.Users on OrderProjectCons.CreatorId equals UserData.CreatorId
                                          where OrderData.UserId == member.id && OrderData.WorkStatus == "價格確認"
                                         select new
                                         {
                                             OrderData.CommissionId,
                                             OrderData.CommissionOrderId,
                                             UserData.UserName,
                                             OrderProjectCons.Title,
                                             OrderData.Price,
                                             OrderData.OrderDate,
                                             OrderData.WorkStatus
                                         });
                            return Json(Order3.ToList());
                        case 5:
                            var Order4 = (from OrderData in _context.CommissionOrders
                                          join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                          join UserData in _context.Users on OrderProjectCons.CreatorId equals UserData.CreatorId
                                          where OrderData.UserId == member.id && OrderData.WorkStatus == "製作中"
                                          select new
                                          {
                                              OrderData.CommissionId,
                                              OrderData.CommissionOrderId,
                                              UserData.UserName,
                                              OrderProjectCons.Title,
                                              OrderData.Price,
                                              OrderData.OrderDate,
                                              OrderData.WorkStatus
                                          });
                            return Json(Order4.ToList());
                        case 6:
                            var Order5 = (from OrderData in _context.CommissionOrders
                                          join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                          join UserData in _context.Users on OrderProjectCons.CreatorId equals UserData.CreatorId
                                          where OrderData.UserId == member.id && OrderData.WorkStatus == "完成"
                                          select new
                                          {
                                              OrderData.CommissionId,
                                              OrderData.CommissionOrderId,
                                              UserData.UserName,
                                              OrderProjectCons.Title,
                                              OrderData.Price,
                                              OrderData.OrderDate,
                                              OrderData.WorkStatus
                                          });
                            return Json(Order5.ToList());
                        case 7:
                            var Order6 = (from OrderData in _context.CommissionOrders
                                          join OrderProjectCons in _context.Commissions on OrderData.CommissionId equals OrderProjectCons.CommissionId
                                          where OrderData.UserId == member.id && OrderData.WorkStatus == "拒絕"
                                          select new
                                          {
                                              OrderData.CommissionId,
                                              OrderProjectCons.Title,
                                              OrderData.Price,
                                              OrderData.OrderDate,
                                              OrderData.WorkStatus,
                                          });
                            return Json(Order6.ToList());
                        default:
                            break;
                    }
                    return Json("Eeeor");
                case "AuthorSettings":
                    var AuthorSettings = (from AuthorSettingsData in _context.Plans
                                          where AuthorSettingsData.CreatorId == CreatorId
                                          orderby AuthorSettingsData.PlanLevel
                                          select new
                                          {
                                              AuthorSettingsData.PlanId,
                                              AuthorSettingsData.PlanName,
                                              AuthorSettingsData.Description
                                          ,
                                              AuthorSettingsData.PlanLevel,
                                              AuthorSettingsData.PlanPrice
                                          });
                    return Json(AuthorSettings.ToList());
                case "WorkData":
                    var WorkData = (from WorkRead in _context.Contents
                                    join UserData in _context.Users on WorkRead.CreatorId equals UserData.CreatorId
                                    join PlanData in _context.Plans on WorkRead.PlanId equals PlanData.PlanId
                                    where WorkRead.CreatorId == CreatorId
                                    select new
                                    { WorkRead.CategoryId, WorkRead.Title, WorkRead.Description, PlanData.PlanLevel, WorkRead.ContentId });
                    return Json(WorkData.ToList());
                case "OrderData1":
                    var OrderProject = (from OrderData in _context.Commissions
                                        where OrderData.CreatorId == CreatorId
                                        select new
                                        {
                                            OrderData.Title
                                        ,
                                            Description = OrderData.Description.Length > 10 ?
                                            OrderData.Description.Substring(0, 10) : OrderData.Description
                                        ,
                                            OrderData.PriceMin,
                                            OrderData.PriceMax
                                        });
                    return Json(OrderProject.ToList());
                case "OrderData2":
                    switch (step)
                    {
                        case 1:
                            var OrderAsk1= (from OrderData in _context.CommissionOrders
                                            join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                            join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                            where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "待確認中"
                                            select new
                                            {
                                                OrderData.CommissionOrderId,
                                                OrderData.Title,
                                                UsrtData.UserName,
                                                OrderData.OrderDate,
                                                OrderData.WorkStatus,
                                                OrderData.Description,
                                                OrderData.Price
                                            });
                            return Json(OrderAsk1.ToList());
                        case 2:
                            var OrderAsk2 = (from OrderData in _context.CommissionOrders
                                            join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                             join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                             where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "價格回覆"
                                             select new
                                            {
                                                 OrderData.CommissionOrderId,
                                                OrderData.Title,
                                                 UsrtData.UserName,
                                                 OrderData.OrderDate,
                                                OrderData.WorkStatus,
                                                 OrderData.Description,
                                                 OrderData.Price,
                                             });
                                return Json(OrderAsk2.ToList());
                        case 3:
                            var OrderAsk3 = (from OrderData in _context.CommissionOrders
                                             join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                             join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                             where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "價格確認"
                                             select new
                                             {
                                                 OrderData.CommissionOrderId,
                                                 OrderData.Title,
                                                 UsrtData.UserName,
                                                 OrderData.OrderDate,
                                                 OrderData.WorkStatus,
                                                 OrderData.Description,
                                                 OrderData.Price
                                             });
                            return Json(OrderAsk3.ToList());
                        case 4:
                            var OrderAsk4 = (from OrderData in _context.CommissionOrders
                                             join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                             join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                             where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "製作中"
                                             select new
                                             {
                                                 OrderData.CommissionOrderId,
                                                 OrderData.Title,
                                                 UsrtData.UserName,
                                                 OrderData.OrderDate,
                                                 OrderData.WorkStatus,
                                                 OrderData.Description,
                                                 OrderData.Price
                                             });
                            return Json(OrderAsk4.ToList());
                        case 5:
                            var OrderAsk5 = (from OrderData in _context.CommissionOrders
                                             join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                             join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                             where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "完成"
                                             select new
                                             {
                                                 OrderData.CommissionOrderId,
                                                 OrderData.Title,
                                                 UsrtData.UserName,
                                                 OrderData.OrderDate,
                                                 OrderData.WorkStatus,
                                                 OrderData.Description,
                                                 OrderData.Price
                                             });
                            return Json(OrderAsk5.ToList());
                        case 6:
                            var OrderAsk6 = (from OrderData in _context.CommissionOrders
                                             join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                             join UsrtData in _context.Users on OrderData.UserId equals UsrtData.UserId
                                             where OrderData2.CreatorId == CreatorId && OrderData.WorkStatus == "拒絕"
                                             select new
                                             {
                                                 OrderData.CommissionOrderId,
                                                 OrderData.Title,
                                                 UsrtData.UserName,
                                                 OrderData.OrderDate,
                                                 OrderData.WorkStatus,
                                                 OrderData.Description,
                                                 OrderData.Price
                                             });
                            return Json(OrderAsk6.ToList());
                        default:
                            break;
                    }
                    return Json("Eeeor");

                case "EventData":
                    var EventRead = (from EventData in _context.Events 
                                     join UserData in _context.Users on EventData.CreatorId equals UserData.CreatorId
                                     where EventData.EventCancel == false&& UserData.UserId==member.id
                                     select new
                                     {
                                         EventData.EventId,
                                         EventData.EventName,
                                         EventData.StartDate,
                                         EventData.EndDate,
                                         Description = EventData.Description.Length > 10 ?
                                            EventData.Description.Substring(0, 10) : EventData.Description
                                     });
                    return Json(EventRead.ToList());
                default:
                    return Json("Eeeor");

            }

        }
        [HttpPost]
        public ActionResult IndividualDataUP([FromBody] List<Dictionary<string, string>> data)
        {
            string UserName = data[0]["value"];
            string Email = data[1]["value"];
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var UesrDataConFirm = (from UesrData in _context.Users
                                   where UesrData.UserId == member.id
                                   select UesrData).FirstOrDefault();
            if (UesrDataConFirm != null)
            {
                UesrDataConFirm.UserName = UserName;
                UesrDataConFirm.Email = Email;
                _context.SaveChanges();

            }
            return Json(new { Name = UserName, Email = Email });


        }
        [HttpPost]
        public ActionResult IndividualAvatarUp(string base64)
        {

            byte[] binaryData = Convert.FromBase64String(base64);
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var AvatarChange = (from AvatarData in _context.Users
                                where AvatarData.UserId == member.id
                                select AvatarData).FirstOrDefault();
            if (AvatarChange != null)
            {
                AvatarChange.Avatar = binaryData;
                _context.SaveChanges();
            }
            return Json(base64);

        }
        [HttpPost]
        public ActionResult PlanDataAdd([FromBody] List<Dictionary<string, string>> data)
        {
            string PlanName = data[0]["value"];
            int PlanPrice = Convert.ToInt32(data[1]["value"]);
            int PlanLevel = Convert.ToInt32(data[2]["value"]);
            string PlanDescription = data[3]["value"];
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var CreatorId = (from AvatarData in _context.Users
                             where AvatarData.UserId == member.id
                             select AvatarData.CreatorId).FirstOrDefault();
            var PlanData = (from UserPlan in _context.Plans
                            where UserPlan.CreatorId == CreatorId
                            select UserPlan);
            var Plancheck = (from Plan in PlanData
                             where Plan.PlanLevel == PlanLevel
                             select Plan).FirstOrDefault();
            if (Plancheck != null)
            {
                Plancheck.PlanName = PlanName;
                Plancheck.PlanLevel = PlanLevel;
                Plancheck.PlanPrice = PlanPrice;
                Plancheck.Description = PlanDescription;
                _context.SaveChanges();
            }
            else
            {
                var PlansDataNew = new Plan
                {
                    PlanName = PlanName,
                    PlanLevel = PlanLevel,
                    PlanPrice = PlanPrice,
                    Description = PlanDescription,
                    CreatorId = (int)CreatorId
                };
                _context.Plans.Add(PlansDataNew);
                _context.SaveChanges();
            }
            var PlanDataCheck = (from UserPlan in _context.Plans
                                 where UserPlan.CreatorId == CreatorId
                                 orderby UserPlan.PlanLevel
                                 select new
                                 {
                                     UserPlan.PlanId,
                                     UserPlan.PlanName,
                                     UserPlan.PlanLevel,
                                     UserPlan.PlanPrice,
                                     UserPlan.Description,
                                 });
            return Json(PlanDataCheck.ToList());
        }
        [HttpPost]
        public ActionResult PlanRowRecord(string type, int id)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var TargetData = (from PlanData in _context.Plans
                              join UserData in _context.Users on PlanData.CreatorId equals UserData.CreatorId
                              where PlanData.PlanId == id
                              orderby PlanData.PlanLevel
                              select PlanData);
            return Json(TargetData.ToList());
        }
        [HttpPost]
        public ActionResult DataDelete(string type, int id)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            switch (type)
            {
                case "Works":
                    var WorkTargetData = (from WorkRead in _context.Contents
                                          join UserData in _context.Users on WorkRead.CreatorId equals UserData.CreatorId
                                          join PlanData in _context.Plans on WorkRead.PlanId equals PlanData.PlanId
                                          where WorkRead.ContentId == id
                                          select WorkRead).FirstOrDefault();

                    if (WorkTargetData != null)
                    {
                        _context.Contents.Remove(WorkTargetData);
                        _context.SaveChanges();
                    }

                    var ReturnWork = (from WorkRead in _context.Contents
                                      join UserData in _context.Users on WorkRead.CreatorId equals UserData.CreatorId
                                      join PlanData in _context.Plans on WorkRead.PlanId equals PlanData.PlanId
                                      where WorkRead.CreatorId == member.id
                                      select new
                                      { WorkRead.CategoryId, WorkRead.Title, WorkRead.Description, PlanData.PlanLevel, WorkRead.ContentId });

                    return Json(ReturnWork.ToList());

                case "Event":
                    var EventTargetData = (from EventData in _context.Events
                                           join UserData in _context.Users on EventData.CreatorId equals UserData.CreatorId
                                           where EventData.EventId == id && EventData.EventCancel == false
                                           select EventData).FirstOrDefault();

                    if (EventTargetData != null)
                    {
                        EventTargetData.EventCancel = true;
                        _context.SaveChanges();
                    }

                    var ReturnEvent = (from EventData in _context.Events
                                       join UserData in _context.Users on EventData.CreatorId equals UserData.CreatorId
                                       where UserData.UserId == member.id && EventData.EventCancel == false
                                       orderby EventData.EventId
                                       select new
                                       {
                                           EventData.EventId,
                                           EventData.EventName,
                                           EventData.StartDate,
                                           EventData.EndDate
                                       });

                    return Json(ReturnEvent.ToList());


                case "Plan":
                    var TargetData = (from PlanData in _context.Plans
                                      join UserData in _context.Users on PlanData.CreatorId equals UserData.CreatorId
                                      where PlanData.PlanId == id
                                      select PlanData).FirstOrDefault();
                    if (TargetData != null)
                    {
                        _context.Plans.Remove(TargetData);
                        _context.SaveChanges();
                    }
                    var ReturnFor = (from PlanData in _context.Plans
                                     join UserData in _context.Users on PlanData.CreatorId equals UserData.CreatorId
                                     where UserData.UserId == member.id
                                     orderby PlanData.PlanLevel
                                     select new { PlanData.PlanId, PlanData.PlanName, PlanData.PlanLevel, PlanData.PlanPrice, PlanData.Description });
                    return Json(ReturnFor.ToList());
            }

            return Json("OK");
        }
        [HttpPost]
        public ActionResult FanStatusReply(int id, string Reply)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var Order = (from OrderData in _context.CommissionOrders
                         where OrderData.CommissionOrderId == id && OrderData.UserId == member.id
                         select OrderData).FirstOrDefault();
            string WorkStatus = Order.WorkStatus.ToString();
            switch (Reply)
            {
                
                case "true":
                    switch (WorkStatus)
                    {
                        case "價格回覆":
                            Order.WorkStatus = "價格確認";
                            _context.SaveChanges();
                            break;
                    }
                    break;
                case "false":
                    switch (WorkStatus)
                    {
                        case "價格回覆":
                            Order.WorkStatus = "拒絕";
                            _context.SaveChanges();
                            break;
                    }
                    break;
            }
            return Json("ok");
        }

        public ActionResult CreatorStatusReply(int id, string Reply,int Price, int step)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var Order = (from OrderData in _context.CommissionOrders
                         where OrderData.CommissionOrderId == id
                         select OrderData).FirstOrDefault();
            string WorkStatus = Order.WorkStatus.ToString();
            switch (Reply)
            {

                case "true":
                    switch (WorkStatus)
                    {
                        case "待確認中":
                            Order.WorkStatus = "價格回覆";
                            Order.Price = Price;
                            _context.SaveChanges();
                            break; 
                        case "價格確認":
                            Order.WorkStatus = "製作中";
                            _context.SaveChanges();
                            break;
                        case "製作中":
                            Order.WorkStatus = "完成";
                            _context.SaveChanges();
                            break;
                    }
                    break;
                case "false":
                    switch (WorkStatus)
                    {
                        case "待確認中":
                            Order.WorkStatus = "拒絕";
                            _context.SaveChanges();
                            break;
                        case "價格確認":
                            Order.WorkStatus = "拒絕";
                            _context.SaveChanges();
                            break;
                        case "製作中":
                            Order.WorkStatus = "拒絕";
                            _context.SaveChanges();
                            break;
                    }
                    break;
            }
            return Json("ok");
        }
        public IActionResult Login()
        {
            ViewBag.MembersOnline = MembersOnline();
            return View();
        }
        [HttpPost]
        public ActionResult Login([FromBody] List<Dictionary<string, string>> data)
        {
            string Email = data[0]["value"];
            string Passwd = data[1]["value"];
            var EmailCheck = (from EmailData in _context.Users
                              where EmailData.Email == Email
                              select true).FirstOrDefault();
            var PasswdCheck = (from PasswdData in _context.Users
                               where PasswdData.Email == Email && PasswdData.Password == Passwd
                               select true).FirstOrDefault();
            var Name = (from NameData in _context.Users
                        where NameData.Email == Email
                        select NameData.UserName).FirstOrDefault();
            var ID = (from IDData in _context.Users
                      where IDData.Email == Email
                      select IDData.UserId).FirstOrDefault();
            if (EmailCheck && PasswdCheck)
            {
                var MemberData = new MemberData();
                string Member = JsonConvert.SerializeObject(MemberData);
                HttpContext.Session.SetString("key", Member);
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                /////////
                member.id = ID;
                member.Name = Name;
                member.Email = Email;
                member.Password = Passwd;
                Member = JsonConvert.SerializeObject(member);
                HttpContext.Session.SetString("key", Member);
                var MemberUi = (from UserData in _context.Users
                                where UserData.UserId == ID
                                select  UserData.Avatar).FirstOrDefault();
                if (MemberUi !=null)
                {
                    Convert.ToBase64String(MemberUi);
                }
            
                return Json(MemberUi);
            }
            else if (!EmailCheck)
            {
                return Json("EmailCheck");
            }
            else
            {
                return Json("PasswdCheck");
            }
        }
        [HttpPost]
        public ActionResult LogOut()
        {
            ViewBag.MembersOnline = MembersOnline();
            HttpContext.Session.Clear();
            return Json("ok");
        }
        public IActionResult Signup()
        {
            ViewBag.MembersOnline = MembersOnline();
            return View();
        }
        [HttpPost]
        public ActionResult SignupData([FromBody] List<Dictionary<string, string>> data)
        {
            string Email = data[0]["value"];
            string UserName = data[1]["value"];
            string UserPasswd = data[2]["value"];
            var EmailCheck = (from UserEmailData in _context.Users
                              where UserEmailData.Email == Email
                              select true).FirstOrDefault();
            var NameCheck = (from UserNameData in _context.Users
                             where UserNameData.UserName == UserName
                             select true).FirstOrDefault();
            if (EmailCheck && NameCheck)
            {
                return Json("Email&NameCheck");
            }
            else if (NameCheck)
            {
                return Json("NameCheck");
            }
            else if (EmailCheck)
            {
                return Json("EmailCheck");
            };
            //目前使用者
           
            MemberData member = new MemberData();
            member.Name = UserName;
            member.Email = Email;
            member.Password = UserPasswd;
            string Member = JsonConvert.SerializeObject(member);
            HttpContext.Session.SetString("key", Member);
            //sql
            var UserData = new User
            {
                UserName = UserName,
                Email = Email,
                Password = UserPasswd
            };
            _context.Users.Add(UserData);
            _context.SaveChanges();

            return Json("CheckOk");
        }

        [HttpPost]
        public ActionResult ChangePwd(User user)
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var  userPwdUpdate = _context.Users.FirstOrDefault(u => u.UserId == member.id);

            if (userPwdUpdate != null)
            {
                userPwdUpdate.Password = user.Password;
                _context.SaveChanges();
                return Ok();
            }

            return BadRequest();

        }
		[HttpPost]
		public ActionResult CreatorIDCheck(bool Check)
		{
			var memberJson = HttpContext.Session.GetString("key");
			MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
			if (Check)
			{
			
				var CreatorID = (from UserData in _context.Users
								 where UserData.UserId == member.id
								 select UserData).FirstOrDefault();
                if (CreatorID != null)
                {
                    Creator CreatorNew = new Creator
                    {
                        Description = "",
                        Notice="",
                    };
                  
                    _context.Add(CreatorNew);
                    _context.SaveChanges();

                    var CreatorIDMax = (from UserData in _context.Creators
                                        select UserData.CreatorId).Max();
                    CreatorID.CreatorId = CreatorIDMax ;
                    _context.SaveChanges();
				}
                return Ok();
			}
			else
			{
                var CreatorID = (from UserData in _context.Users
                                 where UserData.UserId == member.id
                                 select UserData.CreatorId).FirstOrDefault();
                if (CreatorID != null)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

	
		}
		}
	}

	
}
