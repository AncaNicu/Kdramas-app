using KdramasApp_BE.Data;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KdramasApp_BE.Repositories.Classes;

public class UserRepository: IUserRepository
{
    private readonly KdramasDbContext _context;

    public UserRepository(KdramasDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        bool exists = await _context.Users.AnyAsync(x => x.Email == email);
        return exists;
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync
            (x => x.Email == email);
    }
}
