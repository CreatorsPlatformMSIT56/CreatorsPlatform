using CreatorsPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorsPlatform.Controllers;

[ApiController, Route("api/dashboard")]
public class DashboardController(ImaginkContext database) : ApiController(database)
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (UserId == null) return Unauthorized();
        var creator = await CreatorId();
        return Ok(new
        {
            creator = await Database.Creators.Where(c => c.CreatorId == creator)
                .Select(c => new { id = c.CreatorId, c.Description, c.Notice }).FirstOrDefaultAsync(),
            plans = await Database.Plans.Where(p => p.CreatorId == creator).OrderBy(p => p.PlanLevel)
                .Select(p => new { id = p.PlanId, name = p.PlanName, price = p.PlanPrice, level = p.PlanLevel, p.Description }).ToListAsync(),
            works = await Database.Contents.Where(c => c.CreatorId == creator && (c.PullOffDate == null || c.PullOffDate > DateTime.Now))
                .OrderByDescending(c => c.ContentId).Select(c => new { id = c.ContentId, c.Title }).ToListAsync(),
            commissions = await Database.Commissions.Where(c => c.CreatorId == creator)
                .OrderByDescending(c => c.CommissionId).Select(c => new { id = c.CommissionId, c.Title }).ToListAsync(),
            events = await Database.Events.Where(e => e.CreatorId == creator && e.EventCancel != true)
                .OrderByDescending(e => e.EventId).Select(e => new { id = e.EventId, name = e.EventName }).ToListAsync(),
            subscriptions = await Database.Subscriptions.Where(s => s.UserId == UserId).OrderByDescending(s => s.SubscriptionId)
                .Select(s => new { id = s.SubscriptionId, name = s.Plan.PlanName, s.StartDate, s.EndDate, s.CreatorId }).ToListAsync(),
            orders = await Database.CommissionOrders.Where(o => o.UserId == UserId || o.Commission.CreatorId == creator)
                .OrderByDescending(o => o.CommissionOrderId).Select(o => new { id = o.CommissionOrderId, title = o.Commission.Title,
                    o.Price, o.Description, status = o.WorkStatus, o.OrderDate, owner = o.Commission.CreatorId == creator }).ToListAsync()
        });
    }
}
