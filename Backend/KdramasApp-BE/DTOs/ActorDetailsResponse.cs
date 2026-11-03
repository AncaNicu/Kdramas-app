namespace KdramasApp_BE.DTOs;

public class ActorDetailsResponse
{
    public int ActorId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly? BirthDate { get; set; }
    public string? ImageUrl { get; set; }
    public List<KdramaSummaryResponse> Kdramas { get; set; } = new();
}
