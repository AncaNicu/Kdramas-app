namespace KdramasApp_BE.DTOs;

public class APIResponse
{
    public string Message { get; }
    public APIResponse(string message)
    {
        this.Message = message;
    }
}
