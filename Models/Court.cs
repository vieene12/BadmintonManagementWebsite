namespace AquarSmartCourt.Models;

public class Court
{
    public int CourtId { get; set; }
    public string CourtCode { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public decimal HourlyPrice { get; set; }
    public string Status { get; set; } = "Available"; // Available (Đang trống), InUse (Đang sử dụng), Maintenance (Bảo trì)
}
