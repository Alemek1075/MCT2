using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class Stat
{
    public int StatId { get; set; }

    public int? PlayerId { get; set; }

    public int? MatchId { get; set; }

    public int? Kills { get; set; }

    public int? Deaths { get; set; }

    public int? Assists { get; set; }

    public decimal? HsPercentage { get; set; }

    public virtual Match? Match { get; set; }

    public virtual Player? Player { get; set; }
}
