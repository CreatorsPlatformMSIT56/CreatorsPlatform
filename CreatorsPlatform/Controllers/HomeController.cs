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
            public string Name { get; set; } = "Error";
            public string Email { get; set; } = "Error";
            public string Password { get; set; } = "Error";

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
