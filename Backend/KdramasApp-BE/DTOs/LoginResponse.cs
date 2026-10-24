namespace KdramasApp_BE.DTOs;

//the type of response sent to FE
public class LoginResponse
{
    public string Message { get; }
    public string? Token { get; }
    public LoginResponse(string message, string? token = null)
    {
        this.Message = message;
        this.Token = token;
    }
}
