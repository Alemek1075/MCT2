using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int? UserId { get; set; }

    public int? TournamentId { get; set; }

    private DateTime? _purchaseDate;
    public DateTime? PurchaseDate { 
        get => _purchaseDate;
        set => _purchaseDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
    }
        
    public string? Status { get; set; }

    public string? QrCode { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TicketStatus? StatusNavigation { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public virtual User? User { get; set; }
}
