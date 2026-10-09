using Azure.Core;
using KdramasApp_BE.DTOs;
using KdramasApp_BE.Enums;
using KdramasApp_BE.Models;
using KdramasApp_BE.Services.Classes;
using KdramasApp_BE.Services.Interfaces;
using KdramasApp_BE.Repositories.Interfaces;
using Moq;


namespace KdramasApp_BE.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> userRepositoryMock;

    private readonly Mock<IPasswordHasher> hasherMock;

    private readonly IAuthService authService;
    public AuthServiceTests()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        hasherMock = new Mock<IPasswordHasher>();
        authService = new AuthService(
            userRepositoryMock.Object,
            hasherMock.Object);
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
        userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        //build the hasshed pass
        hasherMock
            .Setup(x => x.Hash(request.Password))
            .Returns(hasshedPassword);

        //Act
        var result = await authService.RegisterAsync(request);

        //Assert

        //check if it verified the email
        userRepositoryMock.Verify(
            x => x.EmailExistsAsync(request.Email), 
            Times.Once);

        //check if it hasshed the pass
        hasherMock.Verify(
            x => x.Hash(request.Password),
            Times.Once);

        //check if it added the new user
        userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(user =>
                    user.Name == request.Name &&
                    user.Email == request.Email &&
                    user.HashedPassword == hasshedPassword &&
                    user.Role == UserRole.StandardUser)),
            Times.Once);

        //check if it saved the changes
        userRepositoryMock.Verify(
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
        userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        //Act
        var result = await authService.RegisterAsync(request);

        //Assert

        //check if it verified the email
        userRepositoryMock.Verify(
            x => x.EmailExistsAsync(request.Email),
            Times.Once);

        //check if it hasshed the pass -> never
        hasherMock.Verify(
            x => x.Hash(request.Password),
            Times.Never);

        //check if it added the new user -> never
        userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.IsAny<User>()),
            Times.Never);

        //check if it saved the changes -> never
        userRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        //check the result
        Assert.Equal(RegisterStatus.EmailAlreadyExists, result.Status);
    }
}
