using System.ComponentModel.DataAnnotations;
namespace KdramasApp_BE.DTOs;

public class ActorRequest
{
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = "";
    public DateOnly? BirthDate { get; set; }
    [Url]
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
}
