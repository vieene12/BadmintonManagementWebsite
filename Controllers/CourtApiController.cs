using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
using AquarSmartCourt.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquarSmartCourt.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CourtApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<CourtHub> _hubContext;

    public CourtApiController(ApplicationDbContext context, IHubContext<CourtHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET: api/CourtApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Court>>> GetCourts()
    {
        return await _context.Courts.ToListAsync();
    }

    // GET: api/CourtApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Court>> GetCourt(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return NotFound();
        }
        return court;
    }

    // PUT: api/CourtApi/5
    [Authorize(Roles = "2,3")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCourt(int id, Court court)
    {
        if (id != court.CourtId)
        {
            return BadRequest("ID mismatch");
        }

        var dbCourt = await _context.Courts.FindAsync(id);
        if (dbCourt == null)
        {
            return NotFound();
        }

        dbCourt.CourtName = court.CourtName;
        dbCourt.HourlyPrice = court.HourlyPrice;
        if (!string.IsNullOrEmpty(court.Status))
        {
            dbCourt.Status = court.Status;
        }

        _context.Entry(dbCourt).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CourtExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        return NoContent();
    }

    // POST: api/CourtApi
    [Authorize(Roles = "3")]
    [HttpPost]
    public async Task<ActionResult<Court>> PostCourt(Court court)
    {
        if (string.IsNullOrEmpty(court.CourtCode))
        {
            var count = await _context.Courts.CountAsync();
            court.CourtCode = $"S{count + 1:D2}";
        }

        _context.Courts.Add(court);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourt), new { id = court.CourtId }, court);
    }

    // DELETE: api/CourtApi/5
    [Authorize(Roles = "3")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourt(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return NotFound();
        }

        // Delete associated service orders
        var serviceOrders = await _context.ServiceOrders.Where(so => so.CourtId == id).ToListAsync();
        _context.ServiceOrders.RemoveRange(serviceOrders);

        // Delete associated matchmaking groups & participants
        var matchmakingGroups = await _context.MatchmakingGroups.Where(mg => mg.CourtId == id).ToListAsync();
        foreach (var group in matchmakingGroups)
        {
            var participants = await _context.MatchmakingParticipants.Where(mp => mp.MatchmakingGroupId == group.MatchmakingGroupId).ToListAsync();
            _context.MatchmakingParticipants.RemoveRange(participants);
        }
        _context.MatchmakingGroups.RemoveRange(matchmakingGroups);

        // Delete associated surveillance videos
        var videos = await _context.SurveillanceVideos.Where(v => v.CourtId == id).ToListAsync();
        _context.SurveillanceVideos.RemoveRange(videos);

        // Delete associated invoices
        var invoices = await _context.Invoices.Where(i => i.CourtId == id).ToListAsync();
        _context.Invoices.RemoveRange(invoices);

        // Delete associated bookings
        var bookings = await _context.Bookings.Where(b => b.CourtId == id).ToListAsync();
        _context.Bookings.RemoveRange(bookings);

        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        return NoContent();
    }

    // GET: api/CourtApi/services
    [HttpGet("services")]
    public async Task<ActionResult<IEnumerable<ServiceItem>>> GetServices()
    {
        return await _context.ServiceItems.ToListAsync();
    }

    // PUT: api/CourtApi/services/{id}
    [Authorize(Roles = "3")]
    [HttpPut("services/{id}")]
    public async Task<IActionResult> PutService(int id, ServiceItem item)
    {
        if (id != item.ServiceItemId)
        {
            return BadRequest("ID mismatch");
        }

        var dbItem = await _context.ServiceItems.FindAsync(id);
        if (dbItem == null)
        {
            return NotFound();
        }

        dbItem.ItemName = item.ItemName;
        dbItem.UnitPrice = item.UnitPrice;
        if (!string.IsNullOrEmpty(item.Unit))
        {
            dbItem.Unit = item.Unit;
        }
        if (!string.IsNullOrEmpty(item.Category))
        {
            dbItem.Category = item.Category;
        }

        _context.Entry(dbItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/CourtApi/services
    [Authorize(Roles = "3")]
    [HttpPost("services")]
    public async Task<ActionResult<ServiceItem>> PostService(ServiceItem item)
    {
        _context.ServiceItems.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetServices), new { id = item.ServiceItemId }, item);
    }

    // DELETE: api/CourtApi/services/{id}
    [Authorize(Roles = "3")]
    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var item = await _context.ServiceItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        // Delete associated service orders
        var serviceOrders = await _context.ServiceOrders.Where(so => so.ServiceItemId == id).ToListAsync();
        _context.ServiceOrders.RemoveRange(serviceOrders);

        _context.ServiceItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/CourtApi/order
    [HttpPost("order")]
    public async Task<IActionResult> OrderService([FromBody] OrderRequest request)
    {
        var court = await _context.Courts.FindAsync(request.CourtId);
        if (court == null) return NotFound("Court not found");

        var item = await _context.ServiceItems.FindAsync(request.ServiceItemId);
        if (item == null) return NotFound("Service item not found");

        var today = DateTime.Today;
        var activeBooking = await _context.Bookings
            .Where(b => b.CourtId == request.CourtId && b.Status == "Confirmed" && b.StartTime.Date == today)
            .OrderByDescending(b => b.StartTime)
            .FirstOrDefaultAsync();

        int? bookingId = activeBooking?.BookingId;

        var existing = await _context.ServiceOrders
            .FirstOrDefaultAsync(so => so.CourtId == request.CourtId && so.ServiceItemId == request.ServiceItemId && so.BookingId == bookingId);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            _context.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            var order = new ServiceOrder
            {
                CourtId = request.CourtId,
                ServiceItemId = request.ServiceItemId,
                Quantity = request.Quantity,
                BookingId = bookingId,
                OrderTime = DateTime.Now
            };
            _context.ServiceOrders.Add(order);
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    // GET: api/CourtApi/orders/{courtId}
    [HttpGet("orders/{courtId}")]
    public async Task<IActionResult> GetCourtOrders(int courtId)
    {
        var today = DateTime.Today;
        var activeBooking = await _context.Bookings
            .Where(b => b.CourtId == courtId && b.Status == "Confirmed" && b.StartTime.Date == today)
            .OrderByDescending(b => b.StartTime)
            .FirstOrDefaultAsync();

        int? bookingId = activeBooking?.BookingId;

        var orders = await _context.ServiceOrders
            .Where(so => so.CourtId == courtId && (bookingId == null || so.BookingId == bookingId))
            .Include(so => so.ServiceItem)
            .Select(so => new {
                so.ServiceOrderId,
                so.ServiceItemId,
                ItemName = so.ServiceItem != null ? so.ServiceItem.ItemName : "",
                Unit = so.ServiceItem != null ? so.ServiceItem.Unit : "",
                UnitPrice = so.ServiceItem != null ? (double)so.ServiceItem.UnitPrice : 0.0,
                so.Quantity,
                Total = so.ServiceItem != null ? (double)(so.ServiceItem.UnitPrice * so.Quantity) : 0.0
            })
            .ToListAsync();

        return Ok(orders);
    }

    // DELETE: api/CourtApi/orders/{courtId}
    [Authorize(Roles = "2,3")]
    [HttpDelete("orders/{courtId}")]
    public async Task<IActionResult> ClearCourtOrders(int courtId)
    {
        var today = DateTime.Today;
        var activeBooking = await _context.Bookings
            .Where(b => b.CourtId == courtId && b.Status == "Confirmed" && b.StartTime.Date == today)
            .OrderByDescending(b => b.StartTime)
            .FirstOrDefaultAsync();

        int? bookingId = activeBooking?.BookingId;

        var orders = await _context.ServiceOrders.Where(so => so.CourtId == courtId && (bookingId == null || so.BookingId == bookingId)).ToListAsync();
        _context.ServiceOrders.RemoveRange(orders);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/CourtApi/customer?phone=...
    [HttpGet("customer")]
    public async Task<IActionResult> GetCustomerByPhone([FromQuery] string phone)
    {
        var customer = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone && u.Role == 1);
        if (customer == null)
        {
            customer = new User
            {
                Username = phone,
                Password = "123",
                FullName = "Khách vãng lai mới",
                Role = 1,
                PhoneNumber = phone,
                Position = "Khách Hàng",
                LoyaltyPoints = 0,
                IsActive = true
            };
            _context.Users.Add(customer);
            await _context.SaveChangesAsync();
            return Ok(new { customer.FullName, customer.LoyaltyPoints, message = "Created new profile" });
        }
        return Ok(new { customer.FullName, customer.LoyaltyPoints, message = "Found profile" });
    }

    // GET: api/CourtApi/schedule?date=...
    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule([FromQuery] string? date)
    {
        DateTime targetDate = DateTime.Today;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
        {
            targetDate = parsedDate.Date;
        }

        var startOfDay = targetDate.Date;
        var endOfDay = targetDate.Date.AddDays(1).AddTicks(-1);

        var bookings = await _context.Bookings
            .Where(b => b.StartTime >= startOfDay && b.EndTime <= endOfDay && b.Status == "Confirmed")
            .Select(b => new {
                b.BookingId,
                b.CourtId,
                StartTime = b.StartTime.ToString("HH:mm"),
                EndTime = b.EndTime.ToString("HH:mm"),
                b.CustomerName,
                b.CustomerPhone
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // POST: api/CourtApi/book
    [HttpPost("book")]
    public async Task<IActionResult> BookCourt([FromBody] BookingRequest request)
    {
        var court = await _context.Courts.FindAsync(request.CourtId);
        if (court == null) return NotFound("Court not found");
        if (court.Status == "Maintenance")
        {
            return BadRequest("Sân đang trong quá trình bảo trì, không thể đặt lịch.");
        }

        DateTime today = DateTime.Today;
        DateTime startDateTime;
        DateTime endDateTime;

        try
        {
            var startParts = request.StartTime.Split(':');
            var endParts = request.EndTime.Split(':');
            startDateTime = today.AddHours(int.Parse(startParts[0])).AddMinutes(int.Parse(startParts[1]));
            endDateTime = today.AddHours(int.Parse(endParts[0])).AddMinutes(int.Parse(endParts[1]));
        }
        catch
        {
            return BadRequest("Thời gian bắt đầu hoặc kết thúc không hợp lệ.");
        }

        if (startDateTime >= endDateTime)
        {
            return BadRequest("Thời gian kết thúc phải sau thời gian bắt đầu.");
        }

        bool overlaps = await _context.Bookings.AnyAsync(b =>
            b.CourtId == request.CourtId &&
            b.Status == "Confirmed" &&
            b.StartTime < endDateTime &&
            startDateTime < b.EndTime
        );

        if (overlaps)
        {
            return BadRequest("Khung giờ này đã có người đặt trước!");
        }

        int? userId = null;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.CustomerPhone && u.Role == 1);
        if (user != null)
        {
            userId = user.UserId;
        }

        var booking = new Booking
        {
            CourtId = request.CourtId,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            StartTime = startDateTime,
            EndTime = endDateTime,
            Status = "Confirmed",
            UserId = userId
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var now = DateTime.Now;
        if (startDateTime <= now && now <= endDateTime)
        {
            court.Status = "InUse";
            _context.Entry(court).State = EntityState.Modified;
        }

        if (request.Services != null)
        {
            foreach (var svc in request.Services)
            {
                if (svc.Quantity > 0)
                {
                    var order = new ServiceOrder
                    {
                        CourtId = request.CourtId,
                        ServiceItemId = svc.ServiceItemId,
                        Quantity = svc.Quantity,
                        BookingId = booking.BookingId,
                        OrderTime = DateTime.Now
                    };
                    _context.ServiceOrders.Add(order);
                }
            }
        }

        var randomCode = $"CAM_VID_{new Random().Next(1000, 9999)}";
        var video = new SurveillanceVideo
        {
            VideoCode = randomCode,
            CourtId = request.CourtId,
            BookingId = booking.BookingId,
            StartTime = startDateTime,
            EndTime = endDateTime,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            VideoUrl = $"/videos/sim_{court.CourtCode.ToLower()}.mp4",
            FileSize = $"{new Random().Next(300, 990)}MB",
            Status = "Pending"
        };
        _context.SurveillanceVideos.Add(video);

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        return Ok(new { message = "Booking successful", courtName = court.CourtName, bookingId = booking.BookingId });
    }

    // POST: api/CourtApi/checkout/{courtId}
    [HttpPost("checkout/{courtId}")]
    [Authorize(Roles = "2,3")]
    public async Task<IActionResult> Checkout(int courtId)
    {
        var court = await _context.Courts.FindAsync(courtId);
        if (court == null) return NotFound("Court not found");

        var today = DateTime.Today;
        var activeBooking = await _context.Bookings
            .Where(b => b.CourtId == courtId && b.Status == "Confirmed" && b.StartTime.Date == today)
            .OrderByDescending(b => b.StartTime)
            .FirstOrDefaultAsync();

        string custName = "Khách vãng lai";
        string custPhone = "";
        double playHours = 2.0;
        int? bookingId = null;

        if (activeBooking != null)
        {
            custName = activeBooking.CustomerName;
            custPhone = activeBooking.CustomerPhone;

            // FIX LỖI TẠI ĐÂY: Tính toán trực tiếp trên đối tượng C# cục bộ đã kéo từ DB về RAM
            playHours = (activeBooking.EndTime - activeBooking.StartTime).TotalHours;
            if (playHours <= 0) playHours = 1.0;

            bookingId = activeBooking.BookingId;
            activeBooking.Status = "Completed";
            _context.Entry(activeBooking).State = EntityState.Modified;
        }

        decimal courtFee = (decimal)playHours * court.HourlyPrice;

        var serviceOrders = await _context.ServiceOrders
            .Where(so => so.CourtId == courtId && (bookingId == null || so.BookingId == bookingId))
            .Include(so => so.ServiceItem)
            .ToListAsync();

        decimal serviceFee = 0;
        foreach (var order in serviceOrders)
        {
            if (order.ServiceItem != null)
            {
                serviceFee += order.ServiceItem.UnitPrice * order.Quantity;
            }
        }

        decimal totalAmount = courtFee + serviceFee;

        var invoice = new Invoice
        {
            BookingId = bookingId,
            CourtId = courtId,
            CustomerName = custName,
            CustomerPhone = custPhone,
            PlayHours = playHours,
            CourtFee = courtFee,
            ServiceFee = serviceFee,
            TotalAmount = totalAmount,
            PaymentTime = DateTime.Now,
            Status = "Paid"
        };
        _context.Invoices.Add(invoice);

        if (!string.IsNullOrEmpty(custPhone))
        {
            var customer = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == custPhone && u.Role == 1);
            if (customer != null)
            {
                int pointsAdded = (int)(totalAmount / 10000);
                customer.LoyaltyPoints += pointsAdded;
                _context.Entry(customer).State = EntityState.Modified;
            }
        }

        _context.ServiceOrders.RemoveRange(serviceOrders);

        court.Status = "Available";
        _context.Entry(court).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

        return Ok(new
        {
            message = "Checkout successful",
            invoiceId = invoice.InvoiceId,
            courtFee = (double)courtFee,
            serviceFee = (double)serviceFee,
            totalAmount = (double)totalAmount,
            pointsAdded = (int)(totalAmount / 10000)
        });
    }

    // POST: api/CourtApi/bookings/cancel/{bookingId}
    [HttpPost("bookings/cancel/{bookingId}")]
    [Authorize(Roles = "2,3")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return NotFound("Không tìm thấy thông tin lịch đặt sân.");

        booking.Status = "Cancelled";
        _context.Entry(booking).State = EntityState.Modified;

        var now = DateTime.Now;
        if (booking.StartTime <= now && now <= booking.EndTime)
        {
            var court = await _context.Courts.FindAsync(booking.CourtId);
            if (court != null && court.Status == "InUse")
            {
                court.Status = "Available";
                _context.Entry(court).State = EntityState.Modified;
            }
        }

        var matchingGroup = await _context.MatchmakingGroups
            .FirstOrDefaultAsync(g => g.BookingId == bookingId && g.Status != "Cancelled");
        if (matchingGroup != null)
        {
            matchingGroup.Status = "Cancelled";
            _context.Entry(matchingGroup).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

        return Ok(new { message = "Hủy lịch đặt sân thành công!" });
    }

    // POST: api/CourtApi/bookings/change-court
    [HttpPost("bookings/change-court")]
    [Authorize(Roles = "2,3")]
    public async Task<IActionResult> ChangeCourt([FromBody] ChangeCourtRequest request)
    {
        var booking = await _context.Bookings.FindAsync(request.BookingId);
        if (booking == null) return NotFound("Không tìm thấy thông tin lịch đặt sân.");

        var targetCourt = await _context.Courts.FindAsync(request.NewCourtId);
        if (targetCourt == null) return NotFound("Không tìm thấy sân mới.");

        bool overlaps = await _context.Bookings.AnyAsync(b =>
            b.CourtId == request.NewCourtId &&
            b.BookingId != request.BookingId &&
            b.Status == "Confirmed" &&
            b.StartTime < booking.EndTime &&
            booking.StartTime < b.EndTime
        );

        if (overlaps)
        {
            return BadRequest("Sân mới đã có lịch đặt khác trùng khung giờ!");
        }

        var oldCourtId = booking.CourtId;
        booking.CourtId = request.NewCourtId;
        _context.Entry(booking).State = EntityState.Modified;

        var now = DateTime.Now;
        if (booking.StartTime <= now && now <= booking.EndTime)
        {
            var oldCourt = await _context.Courts.FindAsync(oldCourtId);
            if (oldCourt != null && oldCourt.Status == "InUse")
            {
                oldCourt.Status = "Available";
                _context.Entry(oldCourt).State = EntityState.Modified;
            }

            targetCourt.Status = "InUse";
            _context.Entry(targetCourt).State = EntityState.Modified;
        }

        var matchingGroup = await _context.MatchmakingGroups
            .FirstOrDefaultAsync(g => g.BookingId == request.BookingId && g.Status != "Cancelled");
        if (matchingGroup != null)
        {
            matchingGroup.CourtId = request.NewCourtId;
            _context.Entry(matchingGroup).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

        return Ok(new { message = "Chuyển sân thành công!" });
    }

    private bool CourtExists(int id)
    {
        return _context.Courts.Any(e => e.CourtId == id);
    }
}

public class ChangeCourtRequest
{
    public int BookingId { get; set; }
    public int NewCourtId { get; set; }
}

public class OrderRequest
{
    public int CourtId { get; set; }
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
}

public class BookingRequest
{
    public int CourtId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public List<ServiceOrderItemDto> Services { get; set; } = new();
}

public class ServiceOrderItemDto
{
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
}