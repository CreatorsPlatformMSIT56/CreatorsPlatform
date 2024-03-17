using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult EventContent()
        {
            return View();
        }

        public IActionResult CreateEvent()
        {
            return View();
        }
    }
}
