using System.ComponentModel.DataAnnotations;

namespace NurseScheduler.Api.Data.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } // "Nurse", "HeadNurse"
}