using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public int? UserId { get; set; }

    public int? TeamId { get; set; }

    public virtual ICollection<Stat> Stats { get; set; } = new List<Stat>();

    public virtual Team? Team { get; set; }

    public virtual User? User { get; set; }
}
