using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace CreatorsPlatform.Controllers
{
	public class HomeController : Controller
	{
        public class MemberData
        {
            public int id { get; set; } = 1;
            public string Name { get; set; } = "¬¥¬è";
            public string Email { get; set; } = "Loki@example.com";
            public string Password { get; set; } = "password123";

        };
        private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
            var MemberData = new MemberData();
            string Member= JsonConvert.SerializeObject(MemberData);
			HttpContext.Session.SetString("key", Member);
            var memberJson = HttpContext.Session.GetString("key");
            MemberData member = JsonConvert.DeserializeObject<MemberData>(memberJson);
			Console.WriteLine(member.id);
            Console.WriteLine(member.Name);
            Console.WriteLine(member.Email);
            Console.WriteLine(member.Password);
            return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
