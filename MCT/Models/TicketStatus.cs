using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class TicketStatus
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
