using System;

namespace AquarSmartCourt.Models;

public class SurveillanceVideo
{
    public int VideoId { get; set; }
    public string VideoCode { get; set; } = string.Empty; // e.g. CAM_VID_1092
    public int CourtId { get; set; }
    public int? BookingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string FileSize { get; set; } = "800MB";
    public string Status { get; set; } = "Pending"; // Pending, Saved, Deleted

    // Navigation properties
    public Court? Court { get; set; }
    public Booking? Booking { get; set; }
}
