namespace KdramasApp_BE.Models;

public class Actor
{
    public int ActorId {get; set;}
    public string Name { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public string ImageUrl { get; set; } = "";
    public ICollection<KdramaActor> KdramaActors { get; set; }
        = new List<KdramaActor>();
}
