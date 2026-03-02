using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? TicketId { get; set; }

    public string? TransactionId { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    private DateTime? _paymentDate;
    public DateTime? PaymentDate {
        get => _paymentDate;
        set => _paymentDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

    public virtual PaymentStatus? StatusNavigation { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
