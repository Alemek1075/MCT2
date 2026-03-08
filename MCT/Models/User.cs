using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCT.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Поле Username є обов'язковим.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Поле Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email (має містити @ та домен).")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Поле Password Hash є обов'язковим.")]
    public string? PasswordHash { get; set; }

    [Required(ErrorMessage = "Роль є обов'язковою.")]
    public string? Role { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual UserRole? RoleNavigation { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}