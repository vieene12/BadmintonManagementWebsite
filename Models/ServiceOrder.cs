using System;

namespace AquarSmartCourt.Models;

public class ServiceOrder
{
    public int ServiceOrderId { get; set; }
    public int CourtId { get; set; }
    public int ServiceItemId { get; set; }
    public int Quantity { get; set; }
    public int? BookingId { get; set; }
    public DateTime OrderTime { get; set; } = DateTime.Now;

    // Navigation properties
    public Court? Court { get; set; }
    public ServiceItem? ServiceItem { get; set; }
    public Booking? Booking { get; set; }
}
