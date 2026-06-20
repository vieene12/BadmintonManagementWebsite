using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;

namespace AquarSmartCourt.Controllers;

[Authorize(Roles = "2")]
public class ReceptionistController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReceptionistController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var today = DateTime.Today;
        ViewBag.Courts = _context.Courts.OrderBy(c => c.CourtCode).ToList();
        ViewBag.Services = _context.ServiceItems.ToList();
        ViewBag.TodayBookings = _context.Bookings
            .Where(b => b.Status == "Confirmed" && b.StartTime >= today && b.StartTime < today.AddDays(1))
            .ToList();
        return View();
    }
}
