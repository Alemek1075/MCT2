using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class TournamentStatus
{
    public string StatusName { get; set; } = null!;

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
