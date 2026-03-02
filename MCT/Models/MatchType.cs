using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class MatchType
{
    public string TypeName { get; set; } = null!;

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
}
