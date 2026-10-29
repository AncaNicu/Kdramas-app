using KdramasApp_BE.Models;
using KdramasApp_BE.Enums;
using KdramasApp_BE.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KdramasApp_BE.Data.Seeding;

public class AdminSeeder
{
    private readonly KdramasDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AdminSeeder(
        KdramasDbContext context, 
        IPasswordHasher passwordHasher, 
        IConfiguration configuration)
    {
        this._context = context;
        this._passwordHasher = passwordHasher;
        this._configuration = configuration;
    }

    public async Task SeedAsync()
    {
        //take the data from user-secrets
        var name = _configuration["AdminSeed:Name"];
        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];

        //check if at least one of those data is not present
        //if so, throw exception
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin seed configuration is missing");
        }

        //delete the leading and trailing whitespaces from name and email
        name = name.Trim();
        email = email.Trim();

        //check it there is already a user with the same email
        var existingUser = await _context.Users.AnyAsync(x => x.Email == email);

        if(existingUser)
        {
            Console.WriteLine("There is already an account with this email" +
                " Seed data skiped.");
            return;
        }

        //build the new admin
        var admin = new User
        {
            Name = name,
            Email = email,
            HashedPassword = _passwordHasher.Hash(password),
            Role = UserRole.Admin
        };

        //add the new user to db and save the changes
        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        Console.WriteLine("Admin user added successfully");
    }
}
