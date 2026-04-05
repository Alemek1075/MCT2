using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCT.Models;

public partial class Tournament : IValidatableObject
{
    [Column("tournament_id")]
    public int TournamentId { get; set; }

    [Required]
    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("location")]
    public string? Location { get; set; }

    private DateTime? _startDate;
    [Required]
    [Column("start_date")]
    public DateTime? StartDate
    {
        get => _startDate;
        set => _startDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private DateTime? _endDate;
    [Column("end_date")]
    public DateTime? EndDate
    {
        get => _endDate;
        set => _endDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Required]
    [Range(0, 1000000000)]
    [Column("price")]
    public decimal? Price { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Required]
    [Column("places")]
    [Range(0, int.MaxValue)]
    public int Places { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual TournamentStatus? StatusNavigation { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();

    [NotMapped]
    public List<int> SelectedTeamIds { get; set; } = new List<int>();

    [NotMapped]
    public string CurrentStatus
    {
        get
        {
            var today = DateTime.UtcNow.Date;
            if (!StartDate.HasValue) return "Planned";
            if (today < StartDate.Value.Date) return "Planned";
            if (EndDate.HasValue && today > EndDate.Value.Date) return "Completed";
            return "Ongoing";
        }
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
        {
            yield return new ValidationResult("End Date cannot be earlier than Start Date.", new[] { nameof(EndDate) });
        }
    }
}