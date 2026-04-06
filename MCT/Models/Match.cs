using System;
using System.Collections.Generic;

namespace MCT.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public int? TournamentId { get; set; }

        public int? TeamAId { get; set; }
        public int? TeamBId { get; set; }

        public int? ScoreA { get; set; }
        public int? ScoreB { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string? MatchType { get; set; }
        public int? WinnerId { get; set; }

        public virtual Tournament? Tournament { get; set; }
        public virtual Team? TeamA { get; set; }
        public virtual Team? TeamB { get; set; }
        public virtual Team? Winner { get; set; }
        public virtual MatchType? MatchTypeNavigation { get; set; }
        public virtual ICollection<Stat> Stats { get; set; } = new List<Stat>();
    }
}