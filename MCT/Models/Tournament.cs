using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Tournament
{
    public int TournamentId { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    private DateTime? _startDate;
    public DateTime? StartDate
    {
        get => _startDate;
        set => _startDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private DateTime? _endDate;
    public DateTime? EndDate
    {
        get => _endDate;
        set => _endDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }


    public decimal? Price { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual TournamentStatus? StatusNavigation { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();
}
