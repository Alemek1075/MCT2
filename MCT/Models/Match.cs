using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Match
{
    public int MatchId { get; set; }

    [Required(ErrorMessage = "Поле Турнір є обов'язковим.")]
    public int? TournamentId { get; set; }

    [Required(ErrorMessage = "Поле Команда А є обов'язковим.")]
    public int? TeamAId { get; set; }

    [Required(ErrorMessage = "Поле Команда Б є обов'язковим.")]
    public int? TeamBId { get; set; }

    public int? WinnerId { get; set; }

    private DateTime? _scheduledAt;

    [Required(ErrorMessage = "Поле Дата та час є обов'язковим.")]
    public DateTime? ScheduledAt
    {
        get => _scheduledAt;
        set => _scheduledAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Range(0, int.MaxValue, ErrorMessage = "Рахунок не може бути від'ємним.")]
    public int? ScoreA { get; set; } // Може бути пустим

    [Range(0, int.MaxValue, ErrorMessage = "Рахунок не може бути від'ємним.")]
    public int? ScoreB { get; set; } // Може бути пустим

    [Required(ErrorMessage = "Поле Тип матчу є обов'язковим.")]
    public string? MatchType { get; set; }

    public virtual MatchType? MatchTypeNavigation { get; set; }

    public virtual ICollection<Stat> Stats { get; set; } = new List<Stat>();

    public virtual Team? TeamA { get; set; }

    public virtual Team? TeamB { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual Team? Winner { get; set; }
}