using KdramasApp_BE.DTOs;
using KdramasApp_BE.Enums;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace KdramasApp_BE.Services.Classes;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUserRepository repository, IPasswordHasher hasher)
    {
        this._repository = repository;
        this._hasher = hasher;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
               && (sqlException.Number == 2601 ||
                   sqlException.Number == 2627);
    }

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        //checks if the email alraedy exists
        bool emailExists = await _repository.EmailExistsAsync(request.Email);

        //email already exists => cannot create new user
        if(emailExists)
        {
            return new RegisterResult(RegisterStatus.EmailAlreadyExists);
        }

        //hash the password
        var hashPassword = _hasher.Hash(request.Password);

        //the new user to be created
        User newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            HashedPassword = hashPassword,
            Role = UserRole.StandardUser
        };

        //call the repo prepare adding the new user
        await _repository.AddUserAsync(newUser);

        try
        {
            //save the changes
            await _repository.SaveChangesAsync();
        }
        //in case 2 users try to enter the same email at the same time
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return new RegisterResult(RegisterStatus.EmailAlreadyExists);
        }

        //return the success result to the controller
        return new RegisterResult(RegisterStatus.RegisterSuccessful);
    }
}
