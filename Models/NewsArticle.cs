using System;
using System.ComponentModel.DataAnnotations;

namespace AquarSmartCourt.Models;

public class NewsArticle
{
    [Key]
    public int NewsArticleId { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; } // e.g., "NEW MATCH", "BEST SMASHES", "EVENT"

    [StringLength(50)]
    public string? BadgeColor { get; set; } // e.g., "bg-danger", "bg-primary", "bg-warning"

    public string? ImageUrl { get; set; }

    public string? VideoActionUrl { get; set; } // To trigger the video play modal

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
