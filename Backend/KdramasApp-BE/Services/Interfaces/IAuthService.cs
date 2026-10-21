using KdramasApp_BE.DTOs;
namespace KdramasApp_BE.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request); 
}
