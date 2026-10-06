using KdramasApp_BE.Models;
namespace KdramasApp_BE.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}
