using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AquarSmartCourt.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquarSmartCourt.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SurveillanceApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SurveillanceApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Surveillance/videos
    [HttpGet("videos")]
    public async Task<ActionResult<IEnumerable<object>>> GetVideos()
    {
        var videos = await _context.SurveillanceVideos
            .Include(v => v.Court)
            .OrderByDescending(v => v.StartTime)
            .Select(v => new {
                v.VideoId,
                v.VideoCode,
                CourtName = v.Court != null ? v.Court.CourtName : "",
                TimeRange = $"{v.StartTime:dd/MM/yyyy HH:mm} - {v.EndTime:HH:mm}",
                CustomerInfo = $"{v.CustomerName} ({v.CustomerPhone})",
                v.Status,
                v.FileSize,
                v.VideoUrl
            })
            .ToListAsync();

        return Ok(videos);
    }

    // POST: api/Surveillance/save/{id}
    [HttpPost("save/{id}")]
    public async Task<IActionResult> SaveVideo(int id)
    {
        var video = await _context.SurveillanceVideos.FindAsync(id);
        if (video == null) return NotFound("Video not found");

        video.Status = "Saved";
        _context.Entry(video).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã lưu trữ video và gửi link qua Zalo/Email của khách hàng!" });
    }

    // POST: api/Surveillance/delete/{id}
    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        var video = await _context.SurveillanceVideos.FindAsync(id);
        if (video == null) return NotFound("Video not found");

        video.Status = "Deleted";
        video.VideoUrl = ""; // Clear url
        _context.Entry(video).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa bản ghi video bảo mật theo yêu cầu khách hàng!" });
    }
}
