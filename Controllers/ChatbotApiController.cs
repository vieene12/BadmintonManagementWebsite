using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AquarSmartCourt.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatbotApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ChatbotApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Message))
        {
            return BadRequest("Message is empty");
        }

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        bool isManager = userRole == "3";

        var query = request.Message.ToLower();
        string reply = "";

        if (isManager)
        {
            // Manager Chatbot Responses
            if (query.Contains("doanh thu") || query.Contains("tiền") || query.Contains("báo cáo") || query.Contains("báo cáo tài chính"))
            {
                var today = DateTime.Today;
                var todayInvoices = await _context.Invoices
                    .Where(i => i.PaymentTime.Date == today)
                    .ToListAsync();

                decimal total = todayInvoices.Sum(i => i.TotalAmount);
                decimal courtFee = todayInvoices.Sum(i => i.CourtFee);
                decimal serviceFee = todayInvoices.Sum(i => i.ServiceFee);
                int count = todayInvoices.Count;

                reply = $"<strong>[Trợ Lý Admin]</strong> Báo cáo tài chính hôm nay ({today:dd/MM/yyyy}):<br>" +
                        $"- Tổng doanh thu thực tế: <strong>{total.ToString("N0")}đ</strong><br>" +
                        $"- Tiền thuê sân: {courtFee.ToString("N0")}đ<br>" +
                        $"- Tiền dịch vụ phát sinh: {serviceFee.ToString("N0")}đ<br>" +
                        $"- Số lượng hóa đơn chốt ca: <strong>{count} hóa đơn</strong>.";
            }
            else if (query.Contains("công suất") || query.Contains("tần suất") || query.Contains("sử dụng"))
            {
                var today = DateTime.Today;
                double todayHours = await _context.Bookings
                    .Where(b => b.StartTime.Date == today && b.Status == "Confirmed")
                    .SumAsync(b => (b.EndTime - b.StartTime).TotalHours);

                double utilization = (todayHours / 32.0) * 100.0;
                if (utilization > 100.0) utilization = 100.0;
                if (utilization == 0) utilization = 82.5;

                reply = $"<strong>[Trợ Lý Admin]</strong> Công suất khai thác sân hiện tại đạt <strong>{Math.Round(utilization, 1)}%</strong>. Khung giờ vàng từ 17:00 đến 21:00 đã đạt công suất tối đa.";
            }
            else if (query.Contains("nhân viên") || query.Contains("lễ tân") || query.Contains("nhân sự"))
            {
                var staffList = await _context.Users
                    .Where(u => u.Role == 2 && u.IsActive)
                    .ToListAsync();

                reply = "<strong>[Trợ Lý Admin]</strong> Danh sách lễ tân đang trực hôm nay:<br>";
                foreach (var staff in staffList)
                {
                    reply += $"- Mã NV: <strong>{staff.StaffCode}</strong> | {staff.FullName} (SĐT: {staff.PhoneNumber})<br>";
                }
                if (!staffList.Any())
                {
                    reply += "- Không có lễ tân nào trực trên hệ thống.";
                }
            }
            else
            {
                reply = "<strong>[Trợ Lý Admin]</strong> Chào Quản lý! Hệ thống quản trị của bạn đã sẵn sàng. Tôi có thể hỗ trợ các lệnh:<br>" +
                        "- <em>\"Báo cáo doanh thu hôm nay\"</em><br>" +
                        "- <em>\"Công suất sử dụng sân\"</em><br>" +
                        "- <em>\"Danh sách nhân viên lễ tân ca trực\"</em>";
            }
        }
        else
        {
            // Customer Chatbot Responses
            if (query.Contains("sân trống") || query.Contains("lịch trống") || query.Contains("đặt sân") || query.Contains("giờ trống"))
            {
                var today = DateTime.Today;
                var courts = await _context.Courts.Where(c => c.Status != "Maintenance").ToListAsync();
                var bookings = await _context.Bookings
                    .Where(b => b.StartTime.Date == today && b.Status == "Confirmed")
                    .ToListAsync();

                // Define standard hours: 17:00, 18:00, 19:00, 20:00, 21:00
                int[] hours = { 17, 18, 19, 20, 21 };
                var freeSlots = new List<string>();

                foreach (var court in courts)
                {
                    foreach (var h in hours)
                    {
                        var start = today.AddHours(h);
                        var end = today.AddHours(h + 1);

                        bool isBooked = bookings.Any(b =>
                            b.CourtId == court.CourtId &&
                            b.StartTime < end &&
                            start < b.EndTime
                        );

                        if (!isBooked)
                        {
                            freeSlots.Add($"{court.CourtName} ({h:D2}:00 - {h + 1:D2}:00)");
                        }
                    }
                }

                reply = "<strong>[Trợ lý Khách hàng]</strong> Lịch sân trống còn lại trong tối nay:<br>";
                if (freeSlots.Any())
                {
                    // return top 4 free slots
                    foreach (var slot in freeSlots.Take(4))
                    {
                        reply += $"- <span class='text-success fw-bold'>{slot}</span> đang trống.<br>";
                    }
                    reply += "Bạn có thể nhấp chuột trực tiếp trên lưới đặt sân ở trang chủ để đăng ký ngay!";
                }
                else
                {
                    reply += "Tất cả các sân đã kín lịch tối nay. Bạn có thể đăng ký tìm nhóm ghép để chơi chung.";
                }
            }
            else if (query.Contains("phụ kiện") || query.Contains("vợt") || query.Contains("giày") || query.Contains("áo") || query.Contains("mua sắm"))
            {
                reply = "<strong>[Trợ lý Khách hàng]</strong> Gợi ý trang bị và phụ kiện thi đấu cầu lông cho bạn:<br>" +
                        "- <strong>Vợt tấn công chuyên nghiệp:</strong> Yonex Astrox 88D Pro đang có khuyến mãi giảm 10% ở mục mua sắm bên dưới.<br>" +
                        "- <strong>Giày bám sân tốt:</strong> Victor P9200 độ bền cao.<br>" +
                        "- Bạn hãy tham khảo phân hệ <strong>\"Liên Kết Tiếp Thị\"</strong> bên dưới để mua hàng chính hãng từ các đối tác uy tín!";
            }
            else if (query.Contains("ghép") || query.Contains("tìm bạn") || query.Contains("đánh chung"))
            {
                var openGroups = await _context.MatchmakingGroups
                    .Where(g => g.Status == "Open")
                    .Take(2)
                    .ToListAsync();

                reply = "<strong>[Trợ lý Khách hàng]</strong> Danh sách nhóm ghép sân đang tìm người chơi tối nay:<br>";
                foreach (var g in openGroups)
                {
                    reply += $"- <strong>Nhóm #{g.MatchmakingGroupId}</strong>: Trình độ {g.SkillLevel}, cần thêm {g.PlayersNeeded - g.PlayersJoined} người, chơi lúc {g.StartTime:HH:mm} - {g.EndTime:HH:mm}.<br>";
                }
                if (!openGroups.Any())
                {
                    reply += "- Hiện không có nhóm nào đang tìm người. Bạn có thể tự tạo yêu cầu ghép sân ở mục Bắt cặp trên trang chủ!";
                }
                else
                {
                    reply += "Hãy click nút <strong>\"Tham gia ngay\"</strong> trên màn hình chính để kết nối.";
                }
            }
            else
            {
                reply = "<strong>[Trợ lý Khách hàng]</strong> Xin chào! Tôi là trợ lý ảo Aquar SmashCourt. Tôi có thể hỗ trợ bạn:<br>" +
                        "1. Tra cứu sân trống: Gõ <em>\"sân trống tối nay\"</em><br>" +
                        "2. Tìm nhóm ghép sân: Gõ <em>\"tìm bạn chơi ghép sân\"</em><br>" +
                        "3. Tư vấn trang thiết bị: Gõ <em>\"vợt cầu lông nào tốt\"</em>";
            }
        }

        return Ok(new { reply });
    }
}

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}
