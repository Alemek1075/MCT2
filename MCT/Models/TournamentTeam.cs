using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class TournamentTeam
{
    public int Id { get; set; }

    public int? TournamentId { get; set; }

    public int? TeamId { get; set; }

    public int? Placement { get; set; }

    public virtual Team? Team { get; set; }

    public virtual Tournament? Tournament { get; set; }
}
