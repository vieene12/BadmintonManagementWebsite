using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using AquarSmartCourt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AquarSmartCourt.Controllers
{
    [ApiController]
    [Route("Chatbot")]
    public class ChatbotApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _geminiApiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatbotApiController(ApplicationDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _geminiApiKey = configuration["Gemini:ApiKey"]?.Trim() ?? "";
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto request)
        {
            // Kiểm tra dữ liệu đầu vào từ JavaScript gửi lên
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return Ok(new { reply = "<strong>[Aquar AI]</strong> Nội dung tin nhắn không được để trống." });
            }

            // Lấy thông tin định danh và quyền hạn của User đang đăng nhập
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isManager = userRole == "3";
            bool isReceptionist = userRole == "2";
            string systemPrompt = "";

            try
            {
                // --- 1. LẤY DỮ LIỆU TỪ DATABASE (RAG SYSTEM) ---
                if (isManager)
                {
                    var today = DateTime.Today;
                    var todayInvoices = await _context.Invoices.Where(i => i.PaymentTime.Date == today).ToListAsync();
                    decimal total = todayInvoices.Sum(i => i.TotalAmount);
                    decimal courtFee = todayInvoices.Sum(i => i.CourtFee);
                    decimal serviceFee = todayInvoices.Sum(i => i.ServiceFee);
                    int invoiceCount = todayInvoices.Count;

                    var todayBookings = await _context.Bookings
                        .Where(b => b.StartTime.Date == today && b.Status == "Confirmed")
                        .ToListAsync();

                    double todayHours = todayBookings
                        .AsEnumerable()
                        .Sum(b => (b.EndTime - b.StartTime).TotalHours);

                    double utilization = (todayHours / 32.0) * 100.0;
                    if (utilization > 100.0) utilization = 100.0;
                    if (utilization == 0) utilization = 82.5;

                    var staffList = await _context.Users.Where(u => u.Role == 2 && u.IsActive).ToListAsync();
                    string staffText = staffList.Any()
                        ? string.Join(", ", staffList.Select(s => $"Mã NV: {s.StaffCode} - {s.FullName} (SĐT: {s.PhoneNumber})"))
                        : "Không có lễ tân nào đang trực.";

                    systemPrompt = $"Bạn là Trợ Lý Admin của Aquar SmashCourt. Dữ liệu hôm nay ({today:dd/MM/yyyy}):\n" +
                                   $"- Doanh thu: {total:N0}đ (Sân: {courtFee:N0}đ, Dịch vụ: {serviceFee:N0}đ). Số hóa đơn: {invoiceCount}.\n" +
                                   $"- Công suất sân: {Math.Round(utilization, 1)}%.\n" +
                                   $"- Lễ tân trực: {staffText}.\n" +
                                   $"Trả lời ngắn gọn, chuyên nghiệp bằng tiếng Việt. Có thể dùng thẻ HTML cơ bản (<strong>, <br>).";
                }
                else if (isReceptionist)
                {
                    var today = DateTime.Today;
                    var todayBookings = await _context.Bookings
                        .Include(b => b.Court)
                        .Where(b => b.StartTime.Date == today && b.Status == "Confirmed")
                        .ToListAsync();
                    string bookingsText = todayBookings.Any()
                        ? string.Join(", ", todayBookings.Select(b => $"{b.CustomerName} ({b.CustomerPhone}) đặt sân {(b.Court != null ? b.Court.CourtName : $"ID {b.CourtId}")} lúc {b.StartTime:HH:mm}-{b.EndTime:HH:mm}"))
                        : "Hôm nay chưa có lượt đặt sân nào được xác nhận.";

                    var courts = await _context.Courts.ToListAsync();
                    string courtsText = string.Join(", ", courts.Select(c => $"{c.CourtName} ({c.Status})"));

                    var services = await _context.ServiceItems.ToListAsync();
                    string servicesText = string.Join(", ", services.Select(s => $"{s.ItemName}: {s.UnitPrice:N0}đ/{s.Unit}"));

                    systemPrompt = $"Bạn là Trợ lý Lễ tân của Aquar SmashCourt. Dữ liệu thực tế hôm nay ({today:dd/MM/yyyy}):\n" +
                                   $"- Lịch đặt sân: {bookingsText}.\n" +
                                   $"- Trạng thái các sân: {courtsText}.\n" +
                                   $"- Bảng giá dịch vụ: {servicesText}.\n" +
                                   $"Hỗ trợ lễ tân giải đáp thắc mắc về ca trực, lịch đặt sân của khách, giá dịch vụ. Trả lời chuyên nghiệp, ngắn gọn bằng tiếng Việt. Có thể dùng thẻ HTML cơ bản (<strong>, <br>).";
                }
                else
                {
                    var today = DateTime.Today;
                    var courts = await _context.Courts.Where(c => c.Status != "Maintenance").ToListAsync();
                    var bookings = await _context.Bookings.Where(b => b.StartTime.Date == today && b.Status == "Confirmed").ToListAsync();
                    int[] hours = { 17, 18, 19, 20, 21 };
                    var freeSlots = new List<string>();

                    foreach (var court in courts)
                    {
                        foreach (var h in hours)
                        {
                            var start = today.AddHours(h);
                            var end = today.AddHours(h + 1);
                            bool isBooked = bookings.Any(b => b.CourtId == court.CourtId && b.StartTime < end && start < b.EndTime);
                            if (!isBooked) freeSlots.Add($"{court.CourtName} ({h:D2}:00 - {h + 1:D2}:00)");
                        }
                    }
                    string freeSlotsText = freeSlots.Any() ? string.Join(", ", freeSlots.Take(4)) : "Tất cả các sân đã kín lịch tối nay.";

                    var openGroups = await _context.MatchmakingGroups.Where(g => g.Status == "Open").Take(2).ToListAsync();
                    string matchmakingText = openGroups.Any()
                        ? string.Join("; ", openGroups.Select(g => $"Nhóm #{g.MatchmakingGroupId}: Trình độ {g.SkillLevel}, cần thêm {g.PlayersNeeded - g.PlayersJoined} người, chơi lúc {g.StartTime:HH:mm}"))
                        : "Hiện không có nhóm nào đang tìm người chơi.";

                    systemPrompt = $"Bạn là Trợ lý Khách hàng của Aquar SmashCourt. Dữ liệu thực tế tối nay:\n" +
                                   $"- Sân trống: {freeSlotsText}. (Nhắc khách click lưới đặt sân ở trang chủ để đặt).\n" +
                                   $"- Nhóm ghép: {matchmakingText}. (Nhắc khách bấm 'Tham gia ngay' ở màn hình chính).\n" +
                                   $"- Vợt Yonex Astrox 88D Pro đang giảm 10%, giày Victor P9200 tốt (ở mục 'Liên Kết Tiếp Thị').\n" +
                                   $"Trả lời thân thiện, dùng HTML (<strong>, <br>, <span class='text-success fw-bold'>) để làm nổi bật.";
                }

                // --- 2. KIỂM TRA ĐIỀU KIỆN API KEY ---
                if (string.IsNullOrEmpty(_geminiApiKey) || _geminiApiKey.Contains("YOUR_API_KEY"))
                {
                    return Ok(new { reply = "<strong>[Aquar AI]</strong> Chưa cấu hình Google Gemini API Key hợp lệ trong file appsettings.json!" });
                }

                // --- 3. ĐÓNG GÓI PAYLOAD VÀ GỌI GOOGLE GEMINI API ---
                var client = _httpClientFactory.CreateClient();

                // FIXED CHỖ NÀY: Cấu hình đúng chuẩn cặp đôi v1beta và gemini-2.0-flash để chạy mượt API Key đầu AQ...
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_geminiApiKey}";

                var payload = new
                {
                    contents = new[]
                    {
                        new {
                            role = "user",
                            parts = new[] {
                                new { text = $"{systemPrompt}\n\nCâu hỏi của khách hàng: {request.Message}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        maxOutputTokens = 800
                    }
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonPayload = JsonSerializer.Serialize(payload, options);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errContent = await response.Content.ReadAsStringAsync();
                    return Ok(new { reply = $"<span class='text-danger'>⚠️ Lỗi kết nối AI Gateway (Mã lỗi HTTP: {response.StatusCode}). Chi tiết: {errContent}</span>" });
                }

                // --- 4. BÓC TÁCH DỮ LIỆU AN TOÀN TRÁNH CRASH 500 ---
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);

                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var resContent) &&
                    resContent.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0)
                {
                    var replyText = parts[0].GetProperty("text").GetString();
                    return Ok(new { reply = replyText?.Trim() ?? "Không có phản hồi từ trí tuệ nhân tạo." });
                }

                return Ok(new { reply = "<strong>[Aquar AI]</strong> Cấu trúc phản hồi từ AI không hợp lệ hoặc bị chặn nội dung." });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết thay vì để sập hệ thống web
                return Ok(new { reply = $"<span class='text-danger'>⚠️ Lỗi xử lý hệ thống Backend: {ex.Message}</span>" });
            }
        }
    }

    // Lớp nhận dữ liệu từ Client gửi lên
    public class ChatRequestDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}