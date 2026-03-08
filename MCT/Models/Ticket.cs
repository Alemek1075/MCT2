using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    [Required(ErrorMessage = "Поле Користувач є обов'язковим.")]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "Поле Турнір є обов'язковим.")]
    public int? TournamentId { get; set; }

    private DateTime? _purchaseDate;
    [Required(ErrorMessage = "Поле Дата купівлі є обов'язковим.")]
    public DateTime? PurchaseDate
    {
        get => _purchaseDate;
        set => _purchaseDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }

    [Required(ErrorMessage = "Поле Статус є обов'язковим.")]
    public string? Status { get; set; }

    public string? QrCode { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TicketStatus? StatusNavigation { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual User? User { get; set; }
}