﻿using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static CreatorsPlatform.Controllers.yhuController;

namespace CreatorsPlatform.Controllers
{

    public class HotGuyController : Controller
    {
        private readonly ImaginkContext _context;

        public HotGuyController(ImaginkContext context)
        {
            _context = context;
        }
        //臨時增加
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
        //
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
            //
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
            //

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
            //
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
            //
            return View(@event);
        }
    }
}
