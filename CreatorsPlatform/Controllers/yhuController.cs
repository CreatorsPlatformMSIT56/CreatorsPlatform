using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using NuGet.Protocol;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static CreatorsPlatform.Controllers.HomeController;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
                                   select new {UserData.UserName, Newmesg.Title, Newmesg.Description, Newmesg.ImageUrl, Newmesg.UploadDate })
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
                                       select new { Newmesg.TagId, Newmesg.Description, Newmesg.ImageUrl, Newmesg.UploadDate })
                                       .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5).ToList();
                    return Json(NewmesgData, "newmsg");
                case "subscribemsg":

                    var memberJson = HttpContext.Session.GetString("key");
                    MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
					var UserId = (from UserIdData in _context.Users
								  where UserIdData.Email == member.Email
								  select new { UserIdData.UserId }).FirstOrDefault();
					var SubscriptionDataList = (from SubscriptionData in _context.Subscriptions
                                                where SubscriptionData.UserId == UserId.UserId
												select SubscriptionData.CreatorId);

                    var subscribemsgData = (from Contents in _context.Contents
                                            where SubscriptionDataList.Contains(Contents.CreatorId)
                                            select new { Contents.Title, Contents.Description, Contents.ImageUrl }
                                           ).ToList();
                    return Json(subscribemsgData, "subscribemsg");
                case "eventmsg":
                    var eventmsgData = (from eventmsg in _context.Events
                                        join evenimg in _context.EventImages on eventmsg.EventId equals evenimg.EventId
                                        select new
                                        {
                                            eventmsg.EventName,
                                            eventmsg.Description,
                                            eventmsg.StartDate,
                                            eventmsg.EndDate
                                        }).ToList();
                    return Json(eventmsgData, "eventmsg");
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
                              }).OrderByDescending(item => item.UserLikes).Take(6)).ToList();
			var topCreatorUserIds = DefaultCreatorsData.Select(creatorData => creatorData.UserID).ToList();
            //作者依喜歡數排序並取則頭像等
            var AuthorProfile = (from UsersData in _context.Users
								join Introduction in _context.Creators on UsersData.CreatorId equals Introduction.CreatorId
                                 where (topCreatorUserIds).Contains(Introduction.CreatorId)
                                 select new { UsersData.UserId, UsersData.Avatar, UsersData.UserName, Introduction.Description }
                                     );
			Console.WriteLine(AuthorProfile.ToList().Count);
			//依作者照第一個作者群找作品
			var DefaultContentsData = ((from DefaultContents in _context.Contents
										where DefaultContents.CreatorId == DefaultCreatorsData[0].UserID
                                       select new
                                       {
                                           DefaultContents.CreatorId,
                                           DefaultContents.ImageUrl,
                                           DefaultContents.Title,
                                           DefaultContents.Description,
                                           DefaultContents.UploadDate
                                       }).OrderByDescending(item => item.UploadDate).Take(3));



            Console.WriteLine(DefaultContentsData.ToList().Count); 


			ViewBag.DefaultContentsData = DefaultContentsData.ToList();
            ViewBag.AuthorProfile = AuthorProfile.ToList();
            return View();
		}
        [HttpPost]
        public ActionResult IMAGINK(int x)
        {
            var DefaultCreatorsData =
                             (from DefaultContents in _context.Contents
                              where DefaultContents.CategoryId == x
                              group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                              select new
                              {
                                  UserID = PopularityRranking.Key,
                                  UserLikes = PopularityRranking.Sum(r => r.Likes)
                              }).OrderByDescending(item => item.UserLikes).Take(6);
            var DefaultCreatorsDataList = DefaultCreatorsData.ToList();
            Console.WriteLine(DefaultCreatorsData);
            return Json(DefaultCreatorsDataList);
        }
        [HttpPost]
        public ActionResult IMAGINK(string x)
        {
            var CreatorsID = (from Creators in _context.Users
                              where Creators.UserName == x
                              select Creators);
            var CreatorsList = CreatorsID.ToList();
            var DefaultContentsData = (from DefaultContents in _context.Contents
                                       where DefaultContents.CreatorId == CreatorsList[0].CreatorId
                                       select new
                                       {
                                           DefaultContents.ImageUrl,
                                           DefaultContents.Title,
                                           DefaultContents.Description,
                                           DefaultContents.UploadDate
                                       }).OrderByDescending(item => item.UploadDate).Take(3);
            var DefaultContentsDataList = DefaultContentsData.ToList();
            Console.WriteLine(DefaultContentsData);
            return Json(DefaultContentsData);
        }
        public IActionResult Payment()
		{
			return View();
		}
		public IActionResult EntrustPayment()
		{
			return View();
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
                    bool Planadd = false;
                            var AuthorSettings=(from AuthorSettingsData in _context.Plans
                                                where AuthorSettingsData.CreatorId== member.id
                                                select new { AuthorSettingsData.PlanName,AuthorSettingsData.Description
                                                ,AuthorSettingsData.PlanLevel,AuthorSettingsData.PlanPrice} );
                    return Json(AuthorSettings.ToList());
                default:
                    return Json("Eeeor");

            }
       
        }
        [HttpPost]
        public ActionResult IndividualDataUP([FromBody] List<Dictionary<string, string>> data)
        {
            Console.WriteLine(data);
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);

         return Json(data);

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
            if (EmailCheck && PasswdCheck)
            {
                var memberJson = HttpContext.Session.GetString("key");
                MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
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
