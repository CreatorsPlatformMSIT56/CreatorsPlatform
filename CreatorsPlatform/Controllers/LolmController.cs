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

        
        //public IActionResult EventContent()
        //{
        //    return View();
        //}

        // 新增存 id 的變數，並想把他丟到 View
        public async Task<IActionResult> EventContent(int? id)
        {
            TempData["Eid"] = id;
            var imaginkContext = _context.EventsAndImages;

            // 原本想在 controller 寫 LINQ，但不會傳到 view
            //IEnumerable<EventsAndImage> query = from CCC in imaginkContext
            //                                    where CCC.EventId == id
            //                                    select CCC;

            

            return View(await imaginkContext.ToListAsync());
        }

        public IActionResult CreateEvent()
        {
            return View();
        }

        // 
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
    }
}
