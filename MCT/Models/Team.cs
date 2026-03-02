using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string Name { get; set; } = null!;

    public string ShortCode { get; set; } = null!;

    public string? Region { get; set; }

    public int? MemberCount { get; set; }

    public virtual ICollection<Match> MatchTeamAs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchTeamBs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchWinners { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
}
