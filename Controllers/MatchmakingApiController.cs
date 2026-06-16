using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
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

    public MatchmakingApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Matchmaking/groups
    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<object>>> GetOpenGroups()
    {
        var groups = await _context.MatchmakingGroups
            .Where(g => g.Status == "Open")
            .Select(g => new {
                g.MatchmakingGroupId,
                g.SkillLevel,
                StartTime = g.StartTime.ToString("HH:mm"),
                EndTime = g.EndTime.ToString("HH:mm"),
                g.PlayersNeeded,
                g.PlayersJoined,
                g.CreatorName,
                CourtName = g.Court != null ? g.Court.CourtName : "Chưa xếp sân"
            })
            .ToListAsync();

        return Ok(groups);
    }

    // POST: api/Matchmaking/request
    [HttpPost("request")]
    public async Task<IActionResult> CreateGroup([FromBody] MatchmakingRequestDto request)
    {
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
            return BadRequest("Khung giờ không hợp lệ.");
        }

        if (startDateTime >= endDateTime)
        {
            return BadRequest("Thời gian kết thúc phải sau thời gian bắt đầu.");
        }

        // Find an available court that doesn't overlap for this timeframe
        var courts = await _context.Courts.ToListAsync();
        Court? assignedCourt = null;
        foreach (var court in courts)
        {
            bool overlaps = await _context.Bookings.AnyAsync(b =>
                b.CourtId == court.CourtId &&
                b.Status == "Confirmed" &&
                b.StartTime < endDateTime &&
                startDateTime < b.EndTime
            );
            if (!overlaps)
            {
                assignedCourt = court;
                break;
            }
        }

        if (assignedCourt == null)
        {
            return BadRequest("Không có sân con nào trống trong khung giờ này.");
        }

        var group = new MatchmakingGroup
        {
            SkillLevel = request.SkillLevel,
            StartTime = startDateTime,
            EndTime = endDateTime,
            PlayersNeeded = request.PlayersNeeded,
            PlayersJoined = 1,
            Status = "Open",
            CourtId = assignedCourt.CourtId,
            CreatorName = request.CreatorName
        };

        _context.MatchmakingGroups.Add(group);
        await _context.SaveChangesAsync();

        // Add creator as first participant
        var participant = new MatchmakingParticipant
        {
            MatchmakingGroupId = group.MatchmakingGroupId,
            FullName = request.CreatorName,
            PhoneNumber = request.CreatorPhone,
            JoinedAt = DateTime.Now
        };
        _context.MatchmakingParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo nhóm ghép sân thành công!", groupId = group.MatchmakingGroupId });
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

        // Check if phone number already joined
        bool alreadyJoined = await _context.MatchmakingParticipants.AnyAsync(p =>
            p.MatchmakingGroupId == id && p.PhoneNumber == request.PhoneNumber
        );
        if (alreadyJoined)
        {
            return BadRequest("Số điện thoại này đã tham gia vào nhóm ghép rồi.");
        }

        var participant = new MatchmakingParticipant
        {
            MatchmakingGroupId = id,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            JoinedAt = DateTime.Now
        };
        _context.MatchmakingParticipants.Add(participant);

        group.PlayersJoined += 1;
        _context.Entry(group).State = EntityState.Modified;

        if (group.PlayersJoined >= group.PlayersNeeded)
        {
            group.Status = "Matched";
            // Create the booking automatically!
            var booking = new Booking
            {
                CourtId = group.CourtId ?? 1,
                CustomerName = $"Nhóm ghép - {group.CreatorName}",
                CustomerPhone = request.PhoneNumber, // Representative phone
                StartTime = group.StartTime,
                EndTime = group.EndTime,
                Status = "Confirmed",
                Notes = $"Tự động ghép từ Nhóm #{group.MatchmakingGroupId} ({group.SkillLevel})"
            };
            _context.Bookings.Add(booking);

            // Update Court Status to InUse if current time is within booking slot
            var now = DateTime.Now;
            if (group.StartTime <= now && now <= group.EndTime && group.Court != null)
            {
                group.Court.Status = "InUse";
                _context.Entry(group.Court).State = EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new {
            message = group.Status == "Matched" ? "Ghép nhóm thành công! Sân chơi đã được đặt tự động." : "Tham gia nhóm ghép thành công!",
            status = group.Status,
            playersJoined = group.PlayersJoined
        });
    }
}

public class MatchmakingRequestDto
{
    public string SkillLevel { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int PlayersNeeded { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string CreatorPhone { get; set; } = string.Empty;
}

public class MatchmakingJoinDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
