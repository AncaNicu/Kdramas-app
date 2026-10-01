using KdramasApp_BE.Enums;
namespace KdramasApp_BE.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string HashedPassword { get; set; } = "";
    public UserRole Role { get; set; }

    public ICollection<UserKdrama> UserKdramas { get; set; }
        = new List<UserKdrama>();

}
