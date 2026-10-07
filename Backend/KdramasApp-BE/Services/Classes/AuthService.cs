using KdramasApp_BE.DTOs;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Interfaces;
using KdramasApp_BE.Models;
using KdramasApp_BE.Enums;
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
    public async Task<RegisterResult> RegisterAsync(RegisterRequest request)
    {
        //checks if the email alraedy exists
        bool emailExists = await _repository.EmailExistsAsync(request.Email);

        //email already exists => cannot create new user
        if(emailExists)
        {
            return new RegisterResult(false, "EmailAlreadyExists");
        }

        //hash the password
        var hashPassword = _hasher.Hash(request.Password);

        //the new user to be created
        User newUser = new User();
        newUser.Name = request.Name;
        newUser.Email = request.Email;
        newUser.HashedPassword = hashPassword;
        newUser.Role = UserRole.StandardUser;

        //call the repo prepare adding the new user
        await _repository.AddUserAsync(newUser);

        //save the changes
        await _repository.SaveChangesAsync();

        //return the success result to the controller
        return new RegisterResult(true, "RegisterSuccessful");
    }
}
