using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquarSmartCourt.Models;

namespace AquarSmartCourt.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedUsername = model.Username.Trim();
        var usernameExists = await _context.Users.AnyAsync(u => u.Username == normalizedUsername);
        if (usernameExists)
        {
            ModelState.AddModelError(nameof(model.Username), "Ten dang nhap da ton tai.");
            return View(model);
        }

        var user = new User
        {
            Username = normalizedUsername,
            Password = model.Password,
            FullName = model.FullName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim(),
            Role = 1,
            Position = "Khach Hang"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await SignInUserAsync(user);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToRoleDashboard(user.Role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
            return View();
        }

        await SignInUserAsync(user);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToRoleDashboard(user.Role);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }



    private async Task SignInUserAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("Username", user.Username),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Position", user.Position ?? "Thành viên")
        };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new Claim("PhoneNumber", user.PhoneNumber));
        }

        if (!string.IsNullOrEmpty(user.StaffCode))
        {
            claims.Add(new Claim("StaffCode", user.StaffCode));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private IActionResult RedirectToRoleDashboard(int role)
    {
        return role switch
        {
            1 => RedirectToAction("Index", "Home"),
            2 => RedirectToAction("Index", "Receptionist"),
            3 => RedirectToAction("Index", "Manager"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
