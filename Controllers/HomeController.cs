using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AquarSmartCourt.Models;
using System.Collections.Generic;
using System.Linq;

namespace AquarSmartCourt.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (roleClaim == "2")
        {
            return RedirectToAction("Index", "Receptionist");
        }
        if (roleClaim == "3")
        {
            return RedirectToAction("Index", "Manager");
        }
        if (roleClaim != "1")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var today = DateTime.Today;
        ViewBag.Courts = _context.Courts.OrderBy(c => c.CourtCode).ToList();
        ViewBag.Services = _context.ServiceItems.ToList();
        ViewBag.TodayBookings = _context.Bookings
            .Where(b => b.Status == "Confirmed" && b.StartTime >= today && b.StartTime < today.AddDays(1))
            .ToList();

        var userIdStr = User.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdStr, out int userId))
        {
            var joinedBookingIds = _context.MatchmakingParticipants
                .Where(p => p.UserId == userId && p.MatchmakingGroup != null && p.MatchmakingGroup.BookingId.HasValue)
                .Select(p => p.MatchmakingGroup!.BookingId!.Value)
                .ToList();

            ViewBag.MyBookings = _context.Bookings
                .Include(b => b.Court)
                .Where(b => (b.UserId == userId || joinedBookingIds.Contains(b.BookingId)) && b.Status == "Confirmed" && b.StartTime >= today)
                .OrderBy(b => b.StartTime)
                .ToList();

            var bookingIds = ((List<Booking>)ViewBag.MyBookings).Select(b => b.BookingId).ToList();
            ViewBag.MyMatchmakingGroups = _context.MatchmakingGroups
                .Where(g => g.BookingId.HasValue && bookingIds.Contains(g.BookingId.Value) && g.Status != "Cancelled")
                .ToList();
        }
        else
        {
            ViewBag.MyBookings = new List<Booking>();
            ViewBag.MyMatchmakingGroups = new List<MatchmakingGroup>();
        }

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
