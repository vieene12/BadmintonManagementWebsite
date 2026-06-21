using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
using System;
using System.Threading.Tasks;

namespace AquarSmartCourt.Controllers;

[Authorize(Roles = "3")]
[ApiController]
[Route("api/[controller]")]
public class StaffApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/StaffApi/next-code
    [HttpGet("next-code")]
    public async Task<IActionResult> GetNextStaffCode()
    {
        var staffCodes = await _context.Users
            .Where(u => u.StaffCode != null && u.StaffCode != "")
            .Select(u => u.StaffCode)
            .ToListAsync();

        int maxNumber = 0;
        foreach (var rawCode in staffCodes)
        {
            if (rawCode == null) continue;
            var code = rawCode.Trim();
            if (code.StartsWith("NV", StringComparison.OrdinalIgnoreCase))
            {
                var numPart = code.Substring(2);
                if (int.TryParse(numPart, out int num))
                {
                    if (num > maxNumber)
                    {
                        maxNumber = num;
                    }
                }
            }
        }

        string nextCode = $"NV{(maxNumber + 1).ToString("D3")}";
        return Ok(new { nextCode });
    }

    // POST: api/StaffApi
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username.Trim()))
        {
            return BadRequest("Tên đăng nhập đã tồn tại.");
        }

        var staff = new User
        {
            Username = request.Username.Trim(),
            Password = request.Password,
            FullName = request.FullName.Trim(),
            Role = request.Role,
            StaffCode = request.StaffCode.Trim(),
            DateOfBirth = request.DateOfBirth,
            PhoneNumber = request.PhoneNumber,
            Position = request.Role == 3 ? "Quản lý" : "Lễ tân",
            LoyaltyPoints = 0,
            IsActive = true
        };

        _context.Users.Add(staff);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Thêm nhân viên mới thành công!", staffId = staff.UserId });
    }

    // POST: api/StaffApi/promote
    [HttpPost("promote")]
    public async Task<IActionResult> PromoteCustomer([FromBody] PromoteStaffRequest request)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return NotFound("Không tìm thấy tài khoản người dùng.");
        }

        if (user.Role != 1)
        {
            return BadRequest("Tài khoản này đã có quyền nhân sự hoặc quản trị.");
        }

        user.Role = request.Role;
        user.StaffCode = request.StaffCode.Trim();
        user.DateOfBirth = request.DateOfBirth;
        user.Position = request.Role == 3 ? "Quản lý" : "Lễ tân";

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cấp quyền nhân viên thành công!", staffId = user.UserId });
    }

    // POST: api/StaffApi/demote/{id}
    [HttpPost("demote/{id}")]
    public async Task<IActionResult> DemoteStaff(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("Không tìm thấy nhân viên.");
        }

        if (user.Username == "manager" || (user.Role == 3 && user.StaffCode == "AD001"))
        {
            return BadRequest("Không thể hạ quyền của tài khoản Admin gốc.");
        }

        user.Role = 1;
        user.StaffCode = null;
        user.Position = "Khách Hàng";

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã thu hồi quyền nhân sự thành công!" });
    }
}

public class CreateStaffRequest
{
    public string StaffCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Role { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class PromoteStaffRequest
{
    public int UserId { get; set; }
    public string StaffCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Role { get; set; }
}
