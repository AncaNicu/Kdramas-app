using KdramasApp_BE.Enums;
namespace KdramasApp_BE.Models;

public class KdramaActor
{
    public RoleType RoleType { get; set; }
    //FK to Kdramas
    public int KdramaId { get; set; }
    public Kdrama Kdrama { get; set; } = null!;

    //FK to Actors
    public int ActorId { get; set; }
    public Actor Actor { get; set; } = null!;
}
