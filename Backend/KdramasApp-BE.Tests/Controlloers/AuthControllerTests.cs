using KdramasApp_BE.Services.Interfaces;
using Moq;
using KdramasApp_BE.Enums;
using KdramasApp_BE.DTOs;
using KdramasApp_BE.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace KdramasApp_BE.Tests.Controlloers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> authServiceMock;
    private readonly AuthController authController;

    public AuthControllerTests()
    {
        authServiceMock = new Mock<IAuthService>();
        authController = new AuthController(authServiceMock.Object);
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
        //-> suppose it returns register successful status
        authServiceMock
            .Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(new RegisterResult(RegisterStatus.RegisterSuccessful));

        //Act -> call the controller
        var result = await authController.Register(request);

        //Assert

        //check if the service mock was called once
        authServiceMock.Verify(
            x =>x.RegisterAsync(request),
            Times.Once());

        //check the result type -> it should be an ObjectResult
        var response = Assert.IsType<ObjectResult>(result);

        //chech the status code of the response (we expect a 201 Created)
        Assert.Equal(StatusCodes.Status201Created, response.StatusCode);

        //check the body of the response
        //we expect it to be of type APIResponse
        var apiResponse = Assert.IsType<APIResponse>(response.Value);
        //and its message to be "Registration successful"
        Assert.Equal("Registration successful", apiResponse.Message);
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
        //-> suppose the service response is RegisterStatus.EmailAlreadyExists
        authServiceMock
            .Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(new RegisterResult(RegisterStatus.EmailAlreadyExists));

        //Act
        var result = await authController.Register(request);

        //Assert

        //check if the service was called once
        authServiceMock.Verify
            (x => x.RegisterAsync(request),
            Times.Once());

        //check the result type -> it should be Conflict
        var response = Assert.IsType<ConflictObjectResult>(result);

        //check the response type -> it should be APIResponse
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the message of the api response
        Assert.Equal("There is already an account with the same email", apiResponse.Message);
    }

    [Fact]
    public async Task Register_WhenUnexpectedErrorOccurs()
    {
        //Arrange
        authServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(
            new RegisterResult((RegisterStatus)999)); //creates a default enum element

        //Act
        var result = await authController.Register(It.IsAny<RegisterRequest>());

        //Arrange

        //check if the service was called once
        authServiceMock.Verify
            (x => x.RegisterAsync(It.IsAny<RegisterRequest>()),
            Times.Once);

        //check the type of the response
        var response = Assert.IsType<ObjectResult>(result);

        //check the status code -> we expect a 500
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);

        //check the type of the body
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the message of the response
        Assert.Equal("An unexpected error has occurred", apiResponse.Message);
    }

    //_____________________LOGIN TESTS
}
