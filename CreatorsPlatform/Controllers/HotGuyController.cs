using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers
{

    public class HotGuyController : Controller
    {
        private readonly ImaginkContext _context;

        public HotGuyController(ImaginkContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var imaginkContext = _context.Events.Include(Ex => Ex.Category);

            IEnumerable<CreatorsPlatform.Models.Event> NewEventList = 
                          (from e in imaginkContext
                          orderby e.StartDate descending
                          select e).Take(3);

            IEnumerable<CreatorsPlatform.Models.Event> EndEventList =
                          (from e in imaginkContext
                           orderby e.EndDate
                           select e).Take(3);


            ViewData["NewEvents"] = NewEventList;

            ViewData["OldEvents"] = EndEventList;

            return View(await imaginkContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(Ex => Ex.Category)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }
    }
}
