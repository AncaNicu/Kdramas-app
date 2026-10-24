using KdramasApp_BE.DTOs;
using KdramasApp_BE.Enums;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace KdramasApp_BE.Services.Classes;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _hasher;
    //reference to the JWT settings stored in configuration
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository repository, 
        IPasswordHasher hasher, 
        IConfiguration configuration)
    {
        this._repository = repository;
        this._hasher = hasher;
        this._configuration = configuration;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
               && (sqlException.Number == 2601 ||
                   sqlException.Number == 2627);
    }

    private string GenerateToken(User user)
    {
        string jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT signing key is missing.");

        string jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is missing.");

        string jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is missing.");

        int tokenLifetimeMinutes =
            _configuration.GetValue<int>("Jwt:AccessTokenMinutes");

        if (tokenLifetimeMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT token lifetime must be greater than zero.");
        }

        //the token will contain user id, name and role
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.Name),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(
            Convert.FromBase64String(jwtKey));

        //token is generated with the secret key and 
        //HmacSha256 algorithm
        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            signingCredentials: signingCredentials);

        //serializes the token into a string format
        return new JwtSecurityTokenHandler().WriteToken(token);
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

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        User? user = await _repository.GetUserByEmailAsync(request.Email);
        
        //there is no user with this email
        if(user == null)
        {
            return new LoginResult(LoginStatus.InvalidEmailOrPassword);
        }

        //user exists => check the password
        bool correctPassword = _hasher.Verify(request.Password, user.HashedPassword);
        
        //correct password => return success and the token
        if(correctPassword)
        {
            string token = this.GenerateToken(user);
            return new LoginResult(LoginStatus.LoginSuccessful, token);
        }

        //incorrect password => return invalid email or pass
        return new LoginResult(LoginStatus.InvalidEmailOrPassword);
    }
}
