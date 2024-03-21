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
using CreatorsPlatform.CsMod.yhu;

namespace CreatorsPlatform.Controllers
{
	public class yhuController : Controller
	{
        private readonly ImaginkContext _context;

        public yhuController(ImaginkContext context)
        {
            _context = context;
        }
		private readonly LinqFunctionReserve _LinqGet;

        public yhuController(LinqFunctionReserve LinqGet)
        {
            _LinqGet = LinqGet;
        }



        public class Dftest
		{
			public string? Nickname { get; set; }
			public string? ImageURL { get; set; }
			public string? Title { get; set; }
			public string? Description { get; set; }
		}
		//測試資料
		public List<Dftest> newmsg = new List<Dftest>() {
					 new Dftest { Nickname = "John Doe", ImageURL = "https://example.com/image1.jpg", Title = "Lorem ipsum dolor sit amet", Description = "Consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." },
					 new Dftest { Nickname = "Alice Smith", ImageURL = "https://example.com/image2.jpg", Title = "Duis aute irure dolor in reprehenderit", Description = "Voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
					 new Dftest { Nickname = "Emily Johnson", ImageURL = "https://example.com/image3.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." },
					 new Dftest { Nickname = "Michael Brown", ImageURL = "https://example.com/image4.jpg", Title = "Ut enim ad minim veniam", Description = "Quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." },
					 new Dftest { Nickname = "Jessica Davis", ImageURL = "https://example.com/image5.jpg", Title = "Duis aute irure dolor in reprehenderit", Description = "Voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
					 new Dftest { Nickname = "Daniel Wilson", ImageURL = "https://example.com/image6.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." },
					 new Dftest { Nickname = "Sarah Martinez", ImageURL = "https://example.com/image7.jpg", Title = "Lorem ipsum dolor sit amet", Description = "Consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." },
					 new Dftest { Nickname = "David Taylor", ImageURL = "https://example.com/image8.jpg", Title = "Ut enim ad minim veniam", Description = "Quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." },
					 new Dftest { Nickname = "Jennifer Anderson", ImageURL = "https://example.com/image9.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." },
					 new Dftest { Nickname = "Christopher Thomas", ImageURL = "https://example.com/image10.jpg", Title = "Lorem ipsum dolor sit amet", Description = "Consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." }
				};
		public List<Dftest> subscribemsg = new List<Dftest>() {
					new Dftest { Nickname = "John Doe2tay", ImageURL = "https://example.com/image1.jpg", Title = "Lorem ipsum dolor sit amet", Description = "Consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." },
					new Dftest { Nickname = "Alice Smith2tay", ImageURL = "https://example.com/image2.jpg", Title = "Duis aute irure dolor in reprehenderit", Description = "Voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
					new Dftest { Nickname = "Emily Johnson2tay", ImageURL = "https://example.com/image3.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." },
					new Dftest { Nickname = "Michael Brown2tay", ImageURL = "https://example.com/image4.jpg", Title = "Ut enim ad minim veniam", Description = "Quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." },
					new Dftest { Nickname = "Jessica Davis2tay", ImageURL = "https://example.com/image5.jpg", Title = "Duis aute irure dolor in reprehenderit", Description = "Voluptate velit esse cillum dolore eu fugiat nulla pariatur." },
					new Dftest { Nickname = "Daniel Wilson2tay", ImageURL = "https://example.com/image6.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." },
					new Dftest { Nickname = "Sarah Martinez2tay", ImageURL = "https://example.com/image7.jpg", Title = "Lorem ipsum dolor sit amet", Description = "Consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." },
					new Dftest { Nickname = "David Taylor2tay", ImageURL = "https://example.com/image8.jpg", Title = "Ut enim ad minim veniam", Description = "Quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." },
					new Dftest { Nickname = "Jennifer Anderson2tay", ImageURL = "https://example.com/image9.jpg", Title = "Excepteur sint occaecat cupidatat non proident", Description = "Sunt in culpa qui officia deserunt mollit anim id est laborum." }
				};
		public IActionResult PersonalUser()
		{
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
			var UserPasswd=(from UserPasswdData in _context.Users 
							where UserPasswdData.Email==member.Email
							select new { UserPasswdData .Password}).ToList();
            if (member.Password == UserPasswd[0].Password.ToString())
            {
				var NewmesgData =(from Newmesg in _context.Contents
                                  select new { Newmesg.TagId, Newmesg.Description, Newmesg.ImageUrl,Newmesg.UploadDate}).OrderByDescending(item => item.UploadDate).Take(5);
                return View("PersonalUser", NewmesgData);
            }
			else
			{
                return View("Login");
            };
			
		}
		[HttpPost]
		//public ActionResult PersonalUser(int _CurrentMsg, string tapy)
		//{
		//	switch (tapy)
		//	{
		//		case "newmsg":
		//			var NewmesgData = (from Newmesg in _context.Contents
		//							   select new { Newmesg.TagId, Newmesg.Description, Newmesg.ImageUrl, Newmesg.UploadDate })
		//							   .OrderByDescending(item => item.UploadDate).Skip(_CurrentMsg).Take(5);
		//			var updateList = NewmesgData.ToList();

  //                  break;
		//		case "subscribemsg":
  //                  var memberJson = HttpContext.Session.GetString("key");
  //                  MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
               

  //                  var SubscriptionDataList = (from SubscriptionData in _context.Subscriptions
		//										where SubscriptionData.UserId== _LinqGet.GetUserid(member)




  //                                                  );

  //                  break;
		//	}
		//	//return Json(updateList);
		//}
		public IActionResult IMAGINK()
        {
        //    var ContentsData = 
								//from CreatorsData in  _context.Creators
        //                        join UserData in _context.Users on CreatorsData.CreatorId equals UserData.CreatorId
        //                        select new { UserData.UserName, UserData.Avatar, CreatorsData.CreatorId, CreatorsData.Description };
			var DefaultCreatorsData =
							 (from DefaultContents in _context.Contents
                              group DefaultContents by DefaultContents.CreatorId into PopularityRranking
                              select new
                              {
                                  UserID = PopularityRranking.Key,
                                  UserLikes = PopularityRranking.Sum(r => r.Likes)
                              }).OrderByDescending(item => item.UserLikes).Take(6);
            var DefaultCreatorsDataList = DefaultCreatorsData.ToList();
			Console.WriteLine(DefaultCreatorsData);

            var DefaultContentsData = (from DefaultContents in _context.Contents
									   where DefaultContents.CreatorId == DefaultCreatorsDataList[0].UserID
									   select new { DefaultContents.ImageUrl, DefaultContents.Title,
										   DefaultContents.Description,DefaultContents.UploadDate 
									   }).OrderByDescending(item => item.UploadDate).Take(3);
			var DefaultContentsDataList = DefaultContentsData.ToList();
            Console.WriteLine(DefaultContentsData);


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
							  select  Creators );
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
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }

    }


}
