using KdramasApp_BE.DTOs;
using KdramasApp_BE.Models;
namespace KdramasApp_BE.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request); 
    Task<LoginResult> LoginAsync(LoginRequest request);
}
