using System;

namespace AquarSmartCourt.Models;

public class MatchmakingGroup
{
    public int MatchmakingGroupId { get; set; }
    public string SkillLevel { get; set; } = "Intermediate"; // Beginner, Intermediate, Advanced
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PlayersNeeded { get; set; }
    public int PlayersJoined { get; set; } = 1;
    public string Status { get; set; } = "Open"; // Open, Matched, Cancelled
    public int? CourtId { get; set; }
    public string CreatorName { get; set; } = string.Empty;

    // Navigation properties
    public Court? Court { get; set; }
}
