namespace AquarSmartCourt.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int CourtId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Confirmed";
    public string? Notes { get; set; }
    public int? UserId { get; set; }

    public Court? Court { get; set; }
    public User? User { get; set; }
}
