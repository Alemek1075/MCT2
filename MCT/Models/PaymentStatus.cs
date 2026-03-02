using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class PaymentStatus
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
