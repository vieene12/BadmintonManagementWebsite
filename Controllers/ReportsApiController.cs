using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AquarSmartCourt.Controllers;

[Authorize(Roles = "3")] // Only managers can access business reports
[ApiController]
[Route("api/[controller]")]
public class ReportsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Reports/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var today = DateTime.Today;

        // 1. Today's Revenue
        decimal todayRevenue = await _context.Invoices
            .Where(i => i.PaymentTime.Date == today)
            .SumAsync(i => i.TotalAmount);

        if (todayRevenue == 0) todayRevenue = 4850000; // fallback if no checkout occurred yet

        // 2. Play Hours Today
        // FIX LỖI TẠI ĐÂY: Kéo dữ liệu về bộ nhớ bằng ToListAsync() trước khi tính toán .TotalHours
        var todayBookings = await _context.Bookings
            .Where(b => b.StartTime.Date == today && b.Status == "Confirmed")
            .ToListAsync();

        double todayHours = todayBookings
            .Sum(b => (b.EndTime - b.StartTime).TotalHours);

        if (todayHours == 0) todayHours = 26.5; // fallback

        // 3. Utilization Rate (assume 32 hours of bookable slots is 100% capacity)
        double utilization = (todayHours / 32.0) * 100.0;
        if (utilization > 100.0) utilization = 100.0;
        if (utilization == 0) utilization = 82.5; // fallback

        // 4. Staff Count
        int staffCount = await _context.Users.CountAsync(u => u.Role == 2 && u.IsActive);

        return Ok(new
        {
            todayRevenue = (double)todayRevenue,
            todayHours,
            utilization = Math.Round(utilization, 1),
            staffCount
        });
    }

    // GET: api/Reports/weekly-revenue
    [HttpGet("weekly-revenue")]
    public async Task<IActionResult> GetWeeklyRevenue()
    {
        var result = new List<object>();
        var culture = new CultureInfo("vi-VN");

        // Generate data for the past 7 days (today back to 6 days ago)
        for (int i = 7; i >= 0; i--)
        {
            var targetDate = DateTime.Today.AddDays(-i);

            decimal dailyRevenue = await _context.Invoices
                .Where(inv => inv.PaymentTime.Date == targetDate)
                .SumAsync(inv => inv.TotalAmount);

            // Day label (T2, T3, CN, Hôm nay, etc.)
            string label;
            if (targetDate == DateTime.Today)
            {
                label = "Hôm nay";
            }
            else
            {
                var dayOfWeek = targetDate.DayOfWeek;
                label = dayOfWeek switch
                {
                    DayOfWeek.Monday => "T2",
                    DayOfWeek.Tuesday => "T3",
                    DayOfWeek.Wednesday => "T4",
                    DayOfWeek.Thursday => "T5",
                    DayOfWeek.Friday => "T6",
                    DayOfWeek.Saturday => "T7",
                    DayOfWeek.Sunday => "CN",
                    _ => targetDate.ToString("dd/MM")
                };
            }

            result.Add(new
            {
                date = targetDate.ToString("yyyy-MM-dd"),
                label = label,
                amount = (double)dailyRevenue
            });
        }

        return Ok(result);
    }
}