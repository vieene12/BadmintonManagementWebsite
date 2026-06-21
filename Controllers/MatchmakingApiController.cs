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
public class MatchmakingApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<CourtHub> _hubContext;

    public MatchmakingApiController(ApplicationDbContext context, IHubContext<CourtHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET: api/Matchmaking/groups
    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<object>>> GetOpenGroups()
    {
        var now = DateTime.Now;
        var groups = await _context.MatchmakingGroups
            .Where(g => g.Status == "Open" && g.EndTime > now)
            .Select(g => new {
                g.MatchmakingGroupId,
                g.SkillLevel,
                StartTime = g.StartTime.ToString("HH:mm"),
                EndTime = g.EndTime.ToString("HH:mm"),
                g.PlayersNeeded,
                g.PlayersJoined,
                g.CreatorName,
                CourtName = g.Court != null ? g.Court.CourtName : "Chưa xếp sân",
                Participants = _context.MatchmakingParticipants
                    .Where(p => p.MatchmakingGroupId == g.MatchmakingGroupId)
                    .Select(p => p.FullName)
                    .ToList()
            })
            .ToListAsync();

        return Ok(groups);
    }

    // POST: api/Matchmaking/request
    [HttpPost("request")]
    public async Task<IActionResult> CreateGroup([FromBody] MatchmakingRequestDto request)
    {
        var booking = await _context.Bookings
            .Include(b => b.Court)
            .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

        if (booking == null)
        {
            return NotFound("Không tìm thấy thông tin lịch đặt sân.");
        }

        // Check if this booking already has an active matchmaking group
        var existingGroup = await _context.MatchmakingGroups
            .FirstOrDefaultAsync(g => g.BookingId == request.BookingId && g.Status != "Cancelled");
        if (existingGroup != null)
        {
            return BadRequest("Lịch đặt này đã được đăng ký ghép cặp rồi.");
        }

        var group = new MatchmakingGroup
        {
            BookingId = booking.BookingId,
            SkillLevel = request.SkillLevel,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            PlayersNeeded = request.PlayersNeeded,
            PlayersJoined = 1,
            Status = "Open",
            CourtId = booking.CourtId,
            CreatorName = request.CreatorName
        };

        _context.MatchmakingGroups.Add(group);
        await _context.SaveChangesAsync();

        // Add creator as first participant
        int? creatorUserId = null;
        var creatorUserIdStr = User.FindFirst("UserId")?.Value;
        if (int.TryParse(creatorUserIdStr, out int parsedCreatorUserId))
        {
            creatorUserId = parsedCreatorUserId;
        }

        var participant = new MatchmakingParticipant
        {
            MatchmakingGroupId = group.MatchmakingGroupId,
            FullName = request.CreatorName,
            PhoneNumber = request.CreatorPhone,
            UserId = creatorUserId,
            JoinedAt = DateTime.Now
        };
        _context.MatchmakingParticipants.Add(participant);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

        return Ok(new { message = "Đăng ký ghép sân thành công!", groupId = group.MatchmakingGroupId });
    }

    // POST: api/Matchmaking/join/{id}
    [HttpPost("join/{id}")]
    public async Task<IActionResult> JoinGroup(int id, [FromBody] MatchmakingJoinDto request)
    {
        var group = await _context.MatchmakingGroups
            .Include(g => g.Court)
            .FirstOrDefaultAsync(g => g.MatchmakingGroupId == id);

        if (group == null) return NotFound("Không tìm thấy nhóm ghép.");
        if (group.Status != "Open") return BadRequest("Nhóm ghép này đã đóng hoặc đã bắt cặp xong.");
        if (group.EndTime <= DateTime.Now)
        {
            return BadRequest("Ca chơi của nhóm ghép này đã kết thúc hoặc đã qua.");
        }

        // Check if phone number already joined
        bool alreadyJoined = await _context.MatchmakingParticipants.AnyAsync(p =>
            p.MatchmakingGroupId == id && p.PhoneNumber == request.PhoneNumber
        );
        if (alreadyJoined)
        {
            return BadRequest("Số điện thoại này đã tham gia vào nhóm ghép rồi.");
        }

        // Get current user's UserId for the guest player
        int? guestUserId = null;
        var guestUserIdStr = User.FindFirst("UserId")?.Value;
        if (int.TryParse(guestUserIdStr, out int parsedGuestUserId))
        {
            guestUserId = parsedGuestUserId;
        }

        var participant = new MatchmakingParticipant
        {
            MatchmakingGroupId = id,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            UserId = guestUserId,
            JoinedAt = DateTime.Now
        };
        _context.MatchmakingParticipants.Add(participant);

        group.PlayersJoined += 1;
        _context.Entry(group).State = EntityState.Modified;

        if (group.PlayersJoined >= group.PlayersNeeded)
        {
            group.Status = "Matched";
            
            // Update the existing booking notes instead of creating a new booking
            if (group.BookingId.HasValue)
            {
                var booking = await _context.Bookings.FindAsync(group.BookingId.Value);
                if (booking != null)
                {
                    booking.Notes = $"Đã ghép đủ thành viên ({group.SkillLevel}) - Tổng {group.PlayersJoined} người.";
                    _context.Entry(booking).State = EntityState.Modified;
                }
            }

            // Update Court Status to InUse if current time is within booking slot
            var now = DateTime.Now;
            if (group.StartTime <= now && now <= group.EndTime && group.Court != null)
            {
                group.Court.Status = "InUse";
                _context.Entry(group.Court).State = EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

        return Ok(new {
            message = group.Status == "Matched" ? "Ghép nhóm thành công! Lịch chơi của bạn đã được xác nhận đủ người." : "Tham gia nhóm ghép thành công!",
            status = group.Status,
            playersJoined = group.PlayersJoined
        });
    }
}

public class MatchmakingRequestDto
{
    public int BookingId { get; set; }
    public string SkillLevel { get; set; } = string.Empty;
    public int PlayersNeeded { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string CreatorPhone { get; set; } = string.Empty;
}

public class MatchmakingJoinDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
