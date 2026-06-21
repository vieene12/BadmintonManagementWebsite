using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;

namespace AquarSmartCourt.Controllers;

[Authorize(Roles = "3")]
public class ManagerController : Controller
{
    private readonly ApplicationDbContext _context;

    public ManagerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var today = DateTime.Today;
        ViewBag.Courts = _context.Courts.OrderBy(c => c.CourtCode).ToList();
        ViewBag.Services = _context.ServiceItems.ToList();
        ViewBag.Staffs = _context.Users.Where(u => u.Role == 2 || u.Role == 3).OrderBy(u => u.StaffCode).ToList();
        ViewBag.Customers = _context.Users.Where(u => u.Role == 1).OrderBy(u => u.FullName).ToList();
        ViewBag.TodayBookings = _context.Bookings
            .Where(b => b.Status == "Confirmed" && b.StartTime >= today && b.StartTime < today.AddDays(1))
            .ToList();
        return View();
    }
}
