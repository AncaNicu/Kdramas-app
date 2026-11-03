namespace KdramasApp_BE.DTOs;

public class KdramaSummaryResponse
{
    public int KdramaId { get; set; }
    public string Title { get; set; } = "";
    public int ReleaseYear { get; set; }
    public string? PosterUrl { get; set; }
}
