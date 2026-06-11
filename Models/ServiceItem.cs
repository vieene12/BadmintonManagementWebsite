namespace AquarSmartCourt.Models;

public class ServiceItem
{
    public int ServiceItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // e.g., "Chai", "Cặp/Giờ", "Ống", "Cái"
    public decimal UnitPrice { get; set; }
    public string Category { get; set; } = "Khác"; // e.g., "Nước uống", "Thuê vợt", "Phụ kiện"
}
