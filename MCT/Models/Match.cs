using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int? TournamentId { get; set; }

    public int? TeamAId { get; set; }

    public int? TeamBId { get; set; }

    public int? WinnerId { get; set; }

    private DateTime? _scheduledAt;
    public DateTime? ScheduledAt { 
        get => _scheduledAt;
        set => _scheduledAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }
  

    public int? ScoreA { get; set; }

    public int? ScoreB { get; set; }

    public string? MatchType { get; set; }

    public virtual MatchType? MatchTypeNavigation { get; set; }

    public virtual ICollection<Stat> Stats { get; set; } = new List<Stat>();

    public virtual Team? TeamA { get; set; }

    public virtual Team? TeamB { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual Team? Winner { get; set; }
}
