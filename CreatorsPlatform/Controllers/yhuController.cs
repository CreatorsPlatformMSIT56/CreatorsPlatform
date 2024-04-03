using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Composition;
using System.Diagnostics.Tracing;
using System.Reflection;
using static CreatorsPlatform.Controllers.HomeController;

namespace CreatorsPlatform.Controllers
{
    public class yhuController : Controller
	{
        private readonly ImaginkContext _context;
        public yhuController(ImaginkContext context)
        {
            _context = context;
        }

        public IActionResult PersonalUser()
        {
            var memberJson = HttpContext.Session.GetString("key");
            Console.WriteLine(memberJson);
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var UserPasswd = (from UserPasswdData in _context.Users
                              where UserPasswdData.Email == member.Email
                              select new { UserPasswdData.Password }).ToList();
            if (member.Password == UserPasswd[0].Password.ToString())
            {
                var NewmesgData = (from Newmesg in _context.Contents
                                   join UserData in _context.Users on Newmesg.CreatorId equals UserData.CreatorId
                                   select new { 
                                       Newmesg.Title, 
                                       Newmesg.Description, 
                                       UserData.UserName, 
                                       Newmesg.ImageUrl, 
                                       Newmesg.UploadDate,
                                       Newmesg.ContentId})
                                       .OrderByDescending(item => item.UploadDate).Take(5).ToList();
                ViewBag.NewmesgData = NewmesgData;
                return View("PersonalUser");
            }
            else
            {
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
                                       select new { Newmesg.Title, Newmesg.Description,UserData.UserName,Newmesg.ImageUrl, Newmesg.UploadDate })
                                       .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5).ToList();
                    return Json(NewmesgData.ToList());
                case "subscribemsg":

                    var memberJson = HttpContext.Session.GetString("key");
                    MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
					var SubscriptionDataList = (from ContentsData in _context.Contents
                                                join SubscriptionsData in _context.Subscriptions on ContentsData.CategoryId equals SubscriptionsData.CategoryId
                                                where SubscriptionsData.UserId == member.id
												select SubscriptionsData.CreatorId);

                    var subscribemsgData = (from Contents in _context.Contents
                                            join UserData in _context.Users on Contents.CategoryId equals UserData.CategoryId
                                            where SubscriptionDataList.Contains(Contents.CreatorId)
                                            select new { Contents.Title, Contents.Description, UserData.UserName, Contents.ImageUrl, Contents.UploadDate })
                                       .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5).ToList();
                    return Json(subscribemsgData);
                case "eventmsg":
                    var eventmsgData = (from eventmsg in _context.Events
                                        select new
                                        {
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
                               select new
                               {
                                   UserID = PopularityRranking.Key,
                                   UserLikes = PopularityRranking.Sum(r => r.Likes)
                               }).OrderByDescending(item => item.UserLikes).Take(6).ToList());
            var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.UserID).ToList();
            //作者依喜歡數排序並取則頭像等
            var AuthorProfile = (from UsersData in _context.Users
                                 join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                 where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                 select new { UsersData.UserId,
                                     UsersData.Avatar, 
                                     UsersData.UserName,
									 Description = Introduction.Description.Length > 10 ?
										Introduction.Description.Substring(0, 10) : Introduction.Description
								 });
            Console.WriteLine(AuthorProfile.ToList().Count);
            //依作者照第一個作者群找作品
            var DefaultContentsData = ((from DefaultContents in _context.Contents
                                        where DefaultContents.CreatorId == DefaultCreatorsData[0].UserID
                                        select new
                                        {
                                            DefaultContents.ImageUrl,
                                            DefaultContents.Title,
                                            DefaultContents.UploadDate
                                        }).OrderByDescending(item => item.UploadDate).Take(3));
            var EventDataList = (from EventData in _context.Contents
                                 select new { EventData.ImageUrl });




			Console.WriteLine(DefaultContentsData.ToList().Count);


            ViewBag.DefaultContentsData = DefaultContentsData.ToList();
            ViewBag.AuthorProfile = AuthorProfile.ToList();
            return View();
        }
        //[HttpPost]
        //public ActionResult CreatorsChange(int data)
        //{
        //    var CreatorsData = (from UserData in _context.Users
        //                        join Creators in _context.Creators on UserData.CreatorId equals Creators.CreatorId
        //                        where UserData.CategoryId == data
        //                        select new { UserData.UserId, UserData.Avatar, UserData.UserName, 
        //                            Description = Creators.Description.Length > 10 ? Creators.Description.Substring(0, 10) : Creators.Description 
        //                        });
        //    return Json(CreatorsData.ToList());
        //}
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
                               UserID = PopularityRranking.Key,
                               UserLikes = PopularityRranking.Sum(r => r.Likes)
                           }).OrderByDescending(item => item.UserLikes).Take(6).ToList();
                        var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.UserID).ToList();
                        var AuthorProfile = (from UsersData in _context.Users
                                             join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                             where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                             select new { 
                                                 UsersData.UserId,
												 Avatar = Convert.ToBase64String(UsersData.Avatar), 
                                                 UsersData.UserName,
												 Description = Introduction.Description.Length > 10 ?
										         Introduction.Description.Substring(0, 10) : Introduction.Description
											 }
                                   );
                        return Json(AuthorProfile.ToList());
                    case 8:
                        var NewReportData = (from UserDescription in _context.Creators
											 join NewReport in _context.Users on UserDescription.CreatorId equals NewReport.CreatorId
											 select new { NewReport.UserId,
												 Avatar = Convert.ToBase64String(NewReport.Avatar),
                                                 NewReport.UserName,
                                                 Description = UserDescription.Description.Length > 10 ? UserDescription.Description.Substring(0, 10) : UserDescription.Description
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
                                UserID = PopularityRranking.Key,
                                UserLikes = PopularityRranking.Sum(r => r.Likes)
                            }).OrderByDescending(item => item.UserLikes).Take(6);

                var userIDsArray = CATCreatorsData.Select(data => data.UserID).ToArray();
                var CreatorsData = (from UserData in _context.Users
                                    join Creators in _context.Creators on UserData.CreatorId equals Creators.CreatorId
                                    where (userIDsArray).Contains(UserData.UserId)
                                    select new
                                    {
                                        UserData.UserId,
										Avatar = Convert.ToBase64String(UserData.Avatar),
                                        UserData.UserName,
                                        Description = Creators.Description.Length > 10 ? 
                                        Creators.Description.Substring(0, 10) : Creators.Description
                                    });
                if (CreatorsData== null) {
					var DefaultCreatorsData =
							(from DefaultContents in _context.Contents
							 group DefaultContents by DefaultContents.CreatorId into PopularityRranking
							 select new
							 {
								 UserID = PopularityRranking.Key,
								 UserLikes = PopularityRranking.Sum(r => r.Likes)
							 }).OrderByDescending(item => item.UserLikes).Take(6).ToList();
					var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.UserID).ToList();
					var AuthorProfile = (from UsersData in _context.Users
										 join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
										 where (topCreatorUserIds).Contains(Introduction.CreatorId)
										 select new { 
                                             UsersData.UserId,
                                             UsersData.Avatar,
                                             UsersData.UserName,
											 Description = Introduction.Description.Length > 10 ?
										     Introduction.Description.Substring(0, 10) : Introduction.Description
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
                                           ImageUrl= Convert.ToBase64String(DefaultContents.ImageUrl),
                                           DefaultContents.Title,
                                           DefaultContents.UploadDate
                                       }).OrderByDescending(item => item.UploadDate).Take(3);
            return Json(DefaultContentsData);
        }
        public IActionResult Payment()
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var subPayment = (from p in _context.Plans
                              join i in _context.Users on p.CreatorId equals i.CreatorId
                              where p.PlanId == 3
                              select new
                              {
                                  planId = p.PlanId,
                                  planName = p.PlanName,
                                  planPrice = p.PlanPrice,
                                  planDes = p.Description,
                                  creatorName = i.UserName,
                                  creatorAvatar = i.Avatar
                              });

            ViewBag.subPay = subPayment.ToList();
            ViewBag.Id = member.id;

            return View();
        }


        [HttpPost]
        [Route("yhu/SubPayments")]
        public ActionResult SubPayments(Subscription subPay)
        {
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
                CreatorId = subPay.UserId,
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
        public IActionResult EntrustPayment()
        {
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

            var ComCheck = (from c in _context.Commissions
                            join i in _context.Users on c.CreatorId equals i.CreatorId
                            where c.CommissionId == 1
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

            return View();
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
                UserId = ComOrder.UserId
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
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            var Avatar = (from UserData in _context.Users
                                   where UserData.Email == member.Email
                                   select UserData.Avatar).FirstOrDefault();
            ViewBag.Email = member.Email;
            ViewBag.Name = member.Name;
            ViewBag.Avatar = Convert.ToBase64String(Avatar);
            return View();
        }
        [HttpPost]
        public ActionResult IndividualData(string type)
        {
            var memberJson = HttpContext.Session.GetString("key");
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
                    
                    return Json(new { Email = member.Email, Name = member.Name, Avatar= Convert.ToBase64String(Avatar) });
                case "ConsumptionRecord":
                    var Subscription = (from SubscriptionData in _context.Subscriptions
                                        where SubscriptionData.UserId == member.id
                                        join PlansData in _context.Plans on SubscriptionData.PlanId equals PlansData.PlanId
                                        join UserData in _context.Users on SubscriptionData.CreatorId equals UserData.CreatorId
                                        select new { PlansData.PlanName, UserData.UserName, PlansData.Description
                                        , PlansData.PlanLevel, PlansData.PlanPrice, SubscriptionData.EndDate });
                    //var Commissions = (from CommissionsData in _context.CommissionOrders 
                    //                   where CommissionsData.)
                    return Json(Subscription.ToList());
                case "AuthorSettings":
                            var AuthorSettings=(from AuthorSettingsData in _context.Plans
                                                where AuthorSettingsData.CreatorId== CreatorId
                                                orderby AuthorSettingsData.PlanLevel
                                                select new {AuthorSettingsData.PlanId, AuthorSettingsData.PlanName,AuthorSettingsData.Description
                                                ,AuthorSettingsData.PlanLevel,AuthorSettingsData.PlanPrice} );
                    return Json(AuthorSettings.ToList());
                case "WorkData":               
                    var WorkData = (from WorkRead in _context.Contents
                                    join UserData in _context.Users on WorkRead.CreatorId equals UserData.CreatorId
                                    join PlanData in _context.Plans on WorkRead.PlanId equals PlanData.PlanId
                                    where WorkRead.CreatorId == CreatorId
                                    select new
                                          {WorkRead.CategoryId, WorkRead.Title, WorkRead.Description, PlanData.PlanLevel, WorkRead.ContentId});
                    return Json(WorkData.ToList());
                case "OrderData1":
                    var OrderProject = (from OrderData in _context.Commissions
                                        where OrderData.CreatorId == CreatorId
                                        select new { OrderData.Title
                                        ,
                                            Description = OrderData.Description.Length>10? 
                                            OrderData.Description.Substring(0, 10): OrderData.Description
                                        ,
                                            OrderData.PriceMin,OrderData.PriceMax});
                            return Json( OrderProject.ToList() );
                case "OrderData2":
                    var OrderAsk = (from OrderData in _context.CommissionOrders
                                    join OrderData2 in _context.Commissions on OrderData.CommissionId equals OrderData2.CommissionId
                                    where OrderData2.CreatorId == CreatorId
                                    select new
                                    {
                                        OrderData.Title,
                                        OrderData.OrderDate,
                                        OrderData.DeadlineDate,
                                        OrderData.WorkStatus,
                                        Description = OrderData.Description.Length>10 ?OrderData.Description.Substring(0, 10) : OrderData.Description
                                    });
                    return Json( OrderAsk.ToList() );
                case "EventData":
                    var EventRead = (from EventData in _context.Events
                                     where EventData.EventCancel == false
                                     select new {
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
                UesrDataConFirm.UserName= UserName;
                UesrDataConFirm.Email = Email;
                _context.SaveChanges();

            }
            return Json(new { Name=UserName,Email = Email });


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
                 int  PlanPrice = Convert.ToInt32(data[1]["value"]);
                 int  PlanLevel = Convert.ToInt32(data[2]["value"]);
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
        public ActionResult DataDelete(string type,int id)
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
                                       select new { EventData.EventId, EventData.EventName, EventData.StartDate, EventData.EndDate });

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
                                     select new { PlanData.PlanId,PlanData.PlanName, PlanData.PlanLevel, PlanData.PlanPrice, PlanData.Description });
                    return Json(ReturnFor.ToList());
            }

            return Json("OK");
        }
        
        public IActionResult Login()
        {
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
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
                member.id = ID;
                member.Name = Name;
                member.Email = Email;
                member.Password = Passwd;
                string Member = JsonConvert.SerializeObject(member);
                HttpContext.Session.SetString("key", Member);
                return Json("PersonalUser");
            }
            else if (!EmailCheck) { 
                return Json("EmailCheck");
            } else
            {
                return Json("PasswdCheck");
            }
        }
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignupData([FromBody] List<Dictionary<string, string>> data)
        {
            string Email = data[0]["value"];
            string UserName = data[1]["value"];
            string UserPasswd = data[2]["value"];
            var EmailCheck=( from UserEmailData in _context.Users
                            where UserEmailData.Email == Email
                            select true).FirstOrDefault();
            var NameCheck = (from UserNameData in _context.Users
                             where UserNameData.UserName == UserName
                             select true).FirstOrDefault();
            if(EmailCheck&& NameCheck)
            {
                return Json("Email&NameCheck");
            }else if(NameCheck) {
                return Json("NameCheck");
            }
            else if (EmailCheck)
            {
                return Json("EmailCheck");
            };
            //目前使用者
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            member.Name = UserName;
            member.Email = Email;
            member.Password = UserPasswd;
            string Member = JsonConvert.SerializeObject(member);
            HttpContext.Session.SetString("key", Member);
            //sql
            var UserData= new User { 
            UserName= UserName,
            Email=Email,
            Password=UserPasswd
            };
            _context.Users.Add(UserData);
            _context.SaveChanges();

             return Json("CheckOk");
        }
    }


}
