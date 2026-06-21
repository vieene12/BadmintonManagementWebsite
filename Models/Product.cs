using System;
using System.ComponentModel.DataAnnotations;

namespace AquarSmartCourt.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    [StringLength(50)]
    public string? Badge { get; set; }

    public string? ImageUrl { get; set; }
}
