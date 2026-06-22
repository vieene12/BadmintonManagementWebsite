using System;

namespace AquarSmartCourt.Models;

public class InvoiceDetail
{
    public int InvoiceDetailId { get; set; }
    public int InvoiceId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Invoice? Invoice { get; set; }
}
