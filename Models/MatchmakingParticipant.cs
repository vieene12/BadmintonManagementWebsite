using System;

namespace AquarSmartCourt.Models;

public class MatchmakingParticipant
{
    public int MatchmakingParticipantId { get; set; }
    public int MatchmakingGroupId { get; set; }
    public int? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public MatchmakingGroup? MatchmakingGroup { get; set; }
    public User? User { get; set; }
}
