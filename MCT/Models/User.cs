using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username field is required.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email field is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email format (must contain @ and domain).")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password Hash field is required.")]
    public string? PasswordHash { get; set; }

    [Required(ErrorMessage = "Role field is required.")]
    public string? Role { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    public virtual UserRole? RoleNavigation { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}