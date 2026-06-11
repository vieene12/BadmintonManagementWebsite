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
        ViewBag.Courts = _context.Courts.OrderBy(c => c.CourtCode).ToList();
        ViewBag.Services = _context.ServiceItems.ToList();
        return View();
    }
}
