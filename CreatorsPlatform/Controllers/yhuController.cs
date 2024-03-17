using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static CreatorsPlatform.Controllers.HomeController;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CreatorsPlatform.Controllers
{
	public class yhuController : Controller
	{
		

      

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
		//LINQ方法(TEST)
		public List<Dftest> newsListLinq(int x, int y, List<Dftest> ListName)
		{
			if (ListName.Count < y)
			{
				y = ListName.Count;
			}
			List<Dftest> Form1 = new List<Dftest>();
			for (int i = x; i < y; i++)
			{
				Form1.Add(ListName[i]);
			}
			return Form1;
		}
		public IActionResult PersonalUser()
		{
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
            return View(member);
		}
		[HttpPost]
		public ActionResult PersonalUser(int _CurrentMsg, string tapy)
		{
			List<Dftest> updateList = new List<Dftest>() { };
			switch (tapy)
			{
				case "newmsg":
					updateList = newsListLinq(_CurrentMsg, _CurrentMsg + 10, newmsg);
					break;
				case "subscribemsg":
					updateList = newsListLinq(_CurrentMsg, _CurrentMsg + 10, subscribemsg);
					break;
			}
			return Json(updateList);
		}
		public IActionResult IMAGINK()
		{
			return View();
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
