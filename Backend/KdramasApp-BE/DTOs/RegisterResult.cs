using KdramasApp_BE.Enums;
namespace KdramasApp_BE.DTOs;

public class RegisterResult
{
    public RegisterStatus Status { get; set; }

    public RegisterResult(RegisterStatus status)
    {
        this.Status = status;
    }
}
