namespace KdramasApp_BE.DTOs;

public class RegisterResult
{
    public bool Success { get; set; }
    public string Status { get; set; } = "";

    public RegisterResult(bool succes, string status)
    {
        this.Succes = succes;
        this.Status = status;
    }
}
