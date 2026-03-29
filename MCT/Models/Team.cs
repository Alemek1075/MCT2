using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Team
{
    public int TeamId { get; set; }

    [Required(ErrorMessage = "Поле Назва є обов'язковим.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Поле Короткий код є обов'язковим.")]
    [StringLength(3, MinimumLength = 2, ErrorMessage = "Код має бути 2-3 символи.")]
    public string? ShortCode { get; set; }

    [Required(ErrorMessage = "Поле Регіон є обов'язковим.")]
    public string? Region { get; set; }

    public int? MemberCount { get; set; }

    public virtual ICollection<Match> MatchTeamAs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchTeamBs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchWinners { get; set; } = new List<Match>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
}