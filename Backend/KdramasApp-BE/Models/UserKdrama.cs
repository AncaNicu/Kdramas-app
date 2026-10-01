using KdramasApp_BE.Enums;
namespace KdramasApp_BE.Models;

public class UserKdrama
{
    public string WatchPlatform { get; set; } = "";
    public int WatchedYear { get; set; }
    public Month WatchedMonth { get; set; }

    //FK to Users
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    //FK to Kdramas
    public int KdramaId { get; set; }
    public Kdrama Kdrama { get; set; } = null!;
}
