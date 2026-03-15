using System.Collections.Generic;

namespace MCT.Models
{
    public class LeaderboardViewModel
    {
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? ShortCode { get; set; }
        public int Diff { get; set; }
        public int WonRounds { get; set; }
        public int LostRounds { get; set; }
        public List<Match>? MatchTeamAs { get; set; }
        public List<Match>? MatchTeamBs { get; set; }
    }
}