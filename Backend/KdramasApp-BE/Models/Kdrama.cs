namespace KdramasApp_BE.Models;

public class Kdrama
{
    public int KdramaId { get; set; }
    public string Title { get; set; } = "";
    public int ReleaseYear { get; set; }
    public string BroadcastingNetwork { get; set; } = "";
    public int NoOfEpisodes { get; set; }   
    public string PosterUrl { get; set; } = "";

    public ICollection<KdramaActor> KdramaActors { get; set; }
        = new List<KdramaActor>();

    public ICollection<UserKdrama> UserKdramas { get; set; }
    = new List<UserKdrama>();

}
