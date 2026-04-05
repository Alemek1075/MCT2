using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCT.Models;

public partial class Team
{
    public int TeamId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 2)]
    public string? ShortCode { get; set; }

    [Required]
    public string? Region { get; set; }

    public virtual ICollection<Match> MatchTeamAs { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchTeamBs { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchWinners { get; set; } = new List<Match>();
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();

    [NotMapped]
    public int ActualMemberCount => Players?.Count ?? 0;
}