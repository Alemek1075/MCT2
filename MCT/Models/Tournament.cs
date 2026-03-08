using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Tournament : IValidatableObject
{
    public int TournamentId { get; set; }

    [Required(ErrorMessage = "Поле Опис є обов'язковим.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Поле Локація є обов'язковим.")]
    public string? Location { get; set; }

    private DateTime? _startDate;
    [Required(ErrorMessage = "Поле Дата початку є обов'язковою.")]
    public DateTime? StartDate
    {
        get => _startDate;
        set => _startDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    private DateTime? _endDate;
    [Required(ErrorMessage = "Поле Дата закінчення є обов'язковою.")]
    public DateTime? EndDate
    {
        get => _endDate;
        set => _endDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Required(ErrorMessage = "Поле Ціна є обов'язковим.")]
    [Range(0, 1000000000, ErrorMessage = "Ціна не може бути від'ємною.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Поле Статус є обов'язковим.")]
    public string? Status { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual TournamentStatus? StatusNavigation { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<TournamentTeam> TournamentTeams { get; set; } = new List<TournamentTeam>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && EndDate < StartDate)
        {
            yield return new ValidationResult(
                "Дата завершення не може бути раніше дати початку.",
                new[] { nameof(EndDate) }
            );
        }
    }
}