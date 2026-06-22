using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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

        var invoices = _context.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.Court)
            .Include(invoice => invoice.Details)
            .OrderByDescending(invoice => invoice.PaymentTime)
            .ToList();

        ViewBag.InvoiceHistory = invoices.Select(invoice => new
        {
            code = $"#INV-{invoice.InvoiceId:D6}",
            customerName = invoice.CustomerName,
            courtName = invoice.Court?.CourtName ?? $"Sân #{invoice.CourtId}",
            timeRange = $"{invoice.PaymentTime.AddHours(-invoice.PlayHours):HH:mm} – {invoice.PaymentTime:HH:mm}",
            hours = invoice.PlayHours,
            courtTotal = invoice.CourtFee,
            fnbTotal = invoice.ServiceFee,
            accTotal = 0m,
            discount = 0m,
            grandTotal = invoice.TotalAmount,
            method = "cash",
            status = invoice.Status,
            createdAt = invoice.PaymentTime,
            details = invoice.Details
                .OrderBy(detail => detail.InvoiceDetailId)
                .Select(detail => new
                {
                    itemName = detail.ItemName,
                    category = detail.Category,
                    quantity = detail.Quantity,
                    unitPrice = detail.UnitPrice,
                    lineTotal = detail.LineTotal
                })
                .ToList()
        }).ToList();

        return View();
    }
}
