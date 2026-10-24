using System.ComponentModel.DataAnnotations;

namespace KdramasApp_BE.DTOs;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}
