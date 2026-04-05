using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Tournament : IValidatableObject
{
    public int TournamentId { get; set; }
    [Required] public string? Description { get; set; }
    [Required] public string? Location { get; set; }

    private DateTime? _startDate;
    [Required]
    public DateTime? StartDate
    {
        get => _startDate;
        set => _startDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private DateTime? _endDate;
    [Required]
    public DateTime? EndDate
    {
        get => _endDate;
        set => _endDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Required][Range(0, 1000000000)] public decimal? Price { get; set; }

    [Required][Range(0, 1000000000)] public decimal? Sits { get; set; }
//

    [Required] public string? Status { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual TournamentStatus? StatusNavigation { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
        {
            yield return new ValidationResult("End Date cannot be earlier than Start Date.", new[] { nameof(EndDate) });
        }
    }
}