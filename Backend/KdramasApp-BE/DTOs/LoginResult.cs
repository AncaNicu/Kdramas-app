using KdramasApp_BE.Enums;
namespace KdramasApp_BE.DTOs;

public class LoginResult
{
    public LoginStatus Status { get; }
    public string? Token { get; }
    public LoginResult(LoginStatus status, string? token = null)
    {
        this.Status = status;
        this.Token = token;
    }
}
