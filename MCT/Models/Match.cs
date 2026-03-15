using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Match
{
    public int MatchId { get; set; }
    [Required] public int? TournamentId { get; set; }
    [Required] public int? TeamAId { get; set; }
    [Required] public int? TeamBId { get; set; }
    public int? WinnerId { get; set; }

    private DateTime? _scheduledAt;
    [Required]
    public DateTime? ScheduledAt
    {
        get => _scheduledAt;
        set => _scheduledAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Range(0, int.MaxValue)] public int? ScoreA { get; set; }
    [Range(0, int.MaxValue)] public int? ScoreB { get; set; }
    [Required] public string? MatchType { get; set; }

    public virtual MatchType? MatchTypeNavigation { get; set; }
    public virtual ICollection<Stat> Stats { get; set; } = new List<Stat>();
    public virtual Team? TeamA { get; set; }
    public virtual Team? TeamB { get; set; }
    public virtual Tournament? Tournament { get; set; }
    public virtual Team? Winner { get; set; }
}