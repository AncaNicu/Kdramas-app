using KdramasApp_BE.DTOs;
using KdramasApp_BE.Enums;
using KdramasApp_BE.Models;
using KdramasApp_BE.Services.Classes;
using KdramasApp_BE.Services.Interfaces;
using KdramasApp_BE.Repositories.Interfaces;
using Moq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace KdramasApp_BE.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly IAuthService _authService;
    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();

        //JWT settings used only by tests
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = Convert.ToBase64String(
        System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)),

            ["Jwt:Issuer"] = "KdramasApp.Tests",
            ["Jwt:Audience"] = "KdramasApp.Tests.Api",
            ["Jwt:AccessTokenMinutes"] = "15"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _hasherMock.Object,
            configuration);
    }

    //_____________________REGISTER TESTS
    [Fact]
    public async Task Register_WhenSuccessful()
    {
        //Arrange
        var request = new RegisterRequest
        {
            Name = "John",
            Email = "john@test.com",
            Password = "passSS!123"
        };

        string hasshedPassword = "passwordHash";

        //-> suppose email doesn't already exist
        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        //build the hasshed pass
        _hasherMock
            .Setup(x => x.Hash(request.Password))
            .Returns(hasshedPassword);

        //Act
        var result = await _authService.RegisterAsync(request);

        //Assert

        //check if it verified the email
        _userRepositoryMock.Verify(
            x => x.EmailExistsAsync(request.Email), 
            Times.Once);

        //check if it hasshed the pass
        _hasherMock.Verify(
            x => x.Hash(request.Password),
            Times.Once);

        //check if it added the new user
        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(user =>
                    user.Name == request.Name &&
                    user.Email == request.Email &&
                    user.HashedPassword == hasshedPassword &&
                    user.Role == UserRole.StandardUser)),
            Times.Once);

        //check if it saved the changes
        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        //check the result
        Assert.Equal(RegisterStatus.RegisterSuccessful, result.Status);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists()
    {
        //Arrange
        var request = new RegisterRequest
        {
            Name = "John",
            Email = "john@test.com",
            Password = "passSS!123"
        };

        //-> suppose email already exists
        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        //Act
        var result = await _authService.RegisterAsync(request);

        //Assert

        //check if it verified the email
        _userRepositoryMock.Verify(
            x => x.EmailExistsAsync(request.Email),
            Times.Once);

        //check if it hasshed the pass -> never
        _hasherMock.Verify(
            x => x.Hash(request.Password),
            Times.Never);

        //check if it added the new user -> never
        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.IsAny<User>()),
            Times.Never);

        //check if it saved the changes -> never
        _userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        //check the result
        Assert.Equal(RegisterStatus.EmailAlreadyExists, result.Status);
    }

    //_____________________LOGIN TESTS
    [Fact]
    public async Task Login_WhenSuccessful()
    {
        //Arrange
        var request = new LoginRequest
        {
            Email = "john@email.com",
            Password = "password"
        };

        User user = new User
        {
            UserId = 1,
            Name = "John",
            Email = "john@email.com",
            HashedPassword = "hashed_password",
            Role = UserRole.StandardUser
        };

        //suppose there is a user with the same email
        //and that user is user
        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(user);

        //suppose the password is correct
        _hasherMock
            .Setup(x => x.Verify(request.Password, user.HashedPassword))
            .Returns(true);

        //Act
        var result = await _authService.LoginAsync(request);

        //Assert
        //check if it called GetUserByEmailAsync
        _userRepositoryMock.Verify(
            x => x.GetUserByEmailAsync(request.Email),
            Times.Once);

        //check if it verified the password
        _hasherMock.Verify(
            x => x.Verify(request.Password, user.HashedPassword),
            Times.Once);

        //check the result status
        Assert.Equal(LoginStatus.LoginSuccessful, result.Status);
        //check that the token was generated
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    
        //read the JWT token from the result
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        //check the token claims
        //user ID
        Assert.Equal(
            user.UserId.ToString(),
            token.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier)?.Value);

        //name
        Assert.Equal(
            user.Name,
            token.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.Name)?.Value);

        //role
        Assert.Equal(
            user.Role.ToString(),
            token.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.Role)?.Value);

        //check if expiration date of the token is in the future
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WhenEmailDoesNotExist()
    {
        //Arrange
        var request = new LoginRequest
        {
            Email = "john@test.com",
            Password = "passSS!123"
        };

        User? user = null;

        //suppose there is no user with the email
        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(user);

        //Act
        var result = await _authService.LoginAsync(request);

        //Assert
        //check if the GetUserByEmailAsync was called once
        _userRepositoryMock.Verify(
            x => x.GetUserByEmailAsync(request.Email),
            Times.Once);

        //check if the password was verified -> never
        _hasherMock.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        //check the result
        Assert.Equal(LoginStatus.InvalidEmailOrPassword, result.Status);
    }

    [Fact]
    public async Task Login_WhenWrongPassword()
    {
        //Arrange
        var request = new LoginRequest
        { 
            Email = "john@test.com",
            Password = "wrong_password"
        };

        User user = new User
        {
            Name = "John",
            Email = "john@test.com",
            HashedPassword = "correct_password"
        };

        //suppose the returned user is user
        _userRepositoryMock
            .Setup(x => x.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(user);

        //suppose the paswords don't match
        _hasherMock
            .Setup(x => x.Verify(request.Password, user.HashedPassword))
            .Returns(false);

        //Act
        var result = await _authService.LoginAsync(request);

        //Assert
        //check if GetUserByEmailAsync was called once
        _userRepositoryMock.Verify(
            x => x.GetUserByEmailAsync(request.Email),
            Times.Once);

        //check if it checked the password
        _hasherMock.Verify(
            x => x.Verify(request.Password, user.HashedPassword),
            Times.Once);

        //check the result
        Assert.Equal(LoginStatus.InvalidEmailOrPassword, result.Status);
    }
}
