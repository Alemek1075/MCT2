using System;
using System.Collections.Generic;

namespace MCT.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual UserRole? RoleNavigation { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
