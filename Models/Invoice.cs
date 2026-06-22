using System;
using System.Collections.Generic;

namespace AquarSmartCourt.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int? BookingId { get; set; }
    public int CourtId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public double PlayHours { get; set; }
    public decimal CourtFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PaymentTime { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Paid"; // Paid, Unpaid

    // Navigation properties
    public Booking? Booking { get; set; }
    public Court? Court { get; set; }
    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}
