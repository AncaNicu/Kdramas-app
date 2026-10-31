using KdramasApp_BE.Controllers;
using KdramasApp_BE.DTOs;
using KdramasApp_BE.Models;
using KdramasApp_BE.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KdramasApp_BE.Tests.Controllers;

public class ActorControllerTests
{
    private readonly Mock<IActorService> actorServiceMock;
    private readonly ActorController actorController;

    public ActorControllerTests()
    {
        actorServiceMock = new Mock<IActorService>();
        actorController = new ActorController(actorServiceMock.Object);
    }

    //_____________________ADD AN ACTOR TEST
    [Fact]
    public async Task AddActor_WhenSuccessful()
    {
        //Arrange
        var request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        ActorResponse addedActor = new ActorResponse
        {
            ActorId = 1,
            Name  = request.Name,
            BirthDate = request.BirthDate,
            ImageUrl = request.ImageUrl
        };

        //suppose the service returned addedActor
        actorServiceMock
            .Setup(x => x.AddActorAsync(request))
            .ReturnsAsync(addedActor);

        //Act -> call the controller
        var result = await actorController.AddActor(request);

        //Assert

        //check if the AddActorAsync() of actorServiceMock was called once
        actorServiceMock.Verify(
            x => x.AddActorAsync(request),
            Times.Once);

        //check the result type -> it should be an ObjectResult
        var response = Assert.IsType<ObjectResult>(result);

        //check the status code of the response (201 Created)
        Assert.Equal(StatusCodes.Status201Created, response.StatusCode);

        //check the body of the response to be of type APIResponse
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the message
        Assert.Equal("The actor was added successfully", apiResponse.Message);
    }

    //_____________________GET ALL ACTORS TEST
    [Fact]
    public async Task GetAllActors_WhenSuccessful()
    {
        //Arrange
        ActorResponse actor1 = new ActorResponse
        {
            ActorId = 1,
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        ActorResponse actor2 = new ActorResponse
        {
            ActorId = 2,
            Name = "Brad",
            BirthDate = new DateOnly(1997, 11, 12),
            ImageUrl = "https://image_example.com/"
        };

        List<ActorResponse> actors = new List<ActorResponse>
        {
            actor1, actor2
        };

        //suppose the service returns actors
        actorServiceMock
            .Setup(x => x.GetAllActorsAsync())
            .ReturnsAsync(actors);

        //Act -> call the controller
        var result = await actorController.GetAllActors();

        //Assert

        //check if the service was called once
        actorServiceMock.Verify
            (x => x.GetAllActorsAsync(),
            Times.Once);

        //check the result type
        var response = Assert.IsType<ObjectResult>(result);

        //check the status code -> 200 Ok
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        //check the respones body type
        var actorsResponse = Assert.IsType<List<ActorResponse>>(response.Value);

        //check the number of actors returned
        Assert.Equal(2, actorsResponse.Count);

        //check the actors in the body
        Assert.Equal(actors[0].ActorId, actorsResponse[0].ActorId);
        Assert.Equal(actors[0].Name, actorsResponse[0].Name);
        Assert.Equal(actors[0].BirthDate, actorsResponse[0].BirthDate);
        Assert.Equal(actors[0].ImageUrl, actorsResponse[0].ImageUrl);

        Assert.Equal(actors[1].ActorId, actorsResponse[1].ActorId);
        Assert.Equal(actors[1].Name, actorsResponse[1].Name);
        Assert.Equal(actors[1].BirthDate, actorsResponse[1].BirthDate);
        Assert.Equal(actors[1].ImageUrl, actorsResponse[1].ImageUrl);
    }

    //_____________________GET AN ACTOR BY ID TESTS
    [Fact]
    public async Task GetAnActorById_WhenActorDoesNotExist()
    {
        //Arrange
        int actorId = 1;
        ActorResponse? actor = null;

        //suppose the service returns null when called
        actorServiceMock
            .Setup(x => x.GetActorByIdAsync(actorId))
            .ReturnsAsync(actor);

        //Act
        var result = await actorController.GetActorById(actorId);

        //Assert

        //check the response type
        var response = Assert.IsType<NotFoundObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);

        //check the response body type
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the api response message
        Assert.Equal("The actor was not found", apiResponse.Message);
    }

    [Fact]
    public async Task GetAnActorById_WhenSuccessful()
    {
        //Arrange
        int actorId = 1;
        ActorResponse actor = new ActorResponse
        {
            ActorId = 1,
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        //suppose the service returns actor when called
        actorServiceMock
            .Setup(x => x.GetActorByIdAsync(actorId))
            .ReturnsAsync(actor);

        //Act
        var result = await actorController.GetActorById(actorId);

        //Assert

        //check the response type
        var response = Assert.IsType<OkObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        //check the response body type
        var actorResponse = Assert.IsType<ActorResponse>(response.Value);
    }

    //_____________________GET AN ACTOR BY ID TESTS
    [Fact]
    public async Task DeleteActor_WhenActorDoesNotExist()
    {
        //Arrange
        int actorId = 1;

        //suppose the service returns false when called
        actorServiceMock
            .Setup(x => x.DeleteActorAsync(actorId))
            .ReturnsAsync(false);

        //Act
        var result = await actorController.DeleteActor(actorId);

        //Assert

        //check the response type
        var response = Assert.IsType<ObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);

        //check the response body type
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the api response message
        Assert.Equal("The actor to delete was not found", apiResponse.Message);
    }

    [Fact]
    public async Task DeleteActor_WhenSuccessful()
    {
        //Arrange
        int actorId = 1;

        //suppose the service returns actor when called
        actorServiceMock
            .Setup(x => x.DeleteActorAsync(actorId))
            .ReturnsAsync(true);

        //Act
        var result = await actorController.DeleteActor(actorId);

        //Assert

        //check the response type
        var response = Assert.IsType<ObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        //check the response body type
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the api response message
        Assert.Equal("The actor was deleted successfully", apiResponse.Message);
    }

    //_____________________GET AN ACTOR BY ID TESTS
    [Fact]
    public async Task UpdateActor_WhenActorDoesNotExist()
    {
        //Arrange
        int actorId = 1;

        ActorRequest request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        //suppose the service returns false when called
        actorServiceMock
            .Setup(x => x.UpdateActorAsync(actorId, request))
            .ReturnsAsync(false);

        //Act
        var result = await actorController.UpdateActor(actorId, request);

        //Assert

        //check the response type
        var response = Assert.IsType<ObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);

        //check the response body type
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the api response message
        Assert.Equal("The actor to update was not found", apiResponse.Message);
    }

    [Fact]
    public async Task UpdateActor_WhenSuccessful()
    {
        //Arrange
        int actorId = 1;

        ActorRequest request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        //suppose the service returns true when called
        actorServiceMock
            .Setup(x => x.UpdateActorAsync(actorId, request))
            .ReturnsAsync(true);

        //Act
        var result = await actorController.UpdateActor(actorId, request);

        //Assert

        //check the response type
        var response = Assert.IsType<ObjectResult>(result);

        //check the response status
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        //check the response body type
        var apiResponse = Assert.IsType<APIResponse>(response.Value);

        //check the api response message
        Assert.Equal("The actor was updated successfully", apiResponse.Message);
    }
}
