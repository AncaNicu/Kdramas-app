using System.ComponentModel.DataAnnotations;

namespace KdramasApp_BE.DTOs;

public class RegisterRequest
{
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = "";
    
    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = "";
    
    [Required]
    [MinLength(8)]
    [RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).*$",
    ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one special character.")]
    public string Password { get; set; } = "";
}
