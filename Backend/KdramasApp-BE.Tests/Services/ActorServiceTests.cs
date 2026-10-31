

using Azure;
using KdramasApp_BE.DTOs;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Classes;
using KdramasApp_BE.Services.Interfaces;
using Moq;

namespace KdramasApp_BE.Tests.Services;

public class ActorServiceTests
{
    private readonly Mock<IActorRepository> _actorRepositoryMock;

    private readonly IActorService _actorService;

    public ActorServiceTests()
    {
        _actorRepositoryMock = new Mock<IActorRepository>();

        _actorService = new ActorService(_actorRepositoryMock.Object);
    }

    //_____________________ADD AN ACTOR TEST
    [Fact]
    public async Task AddAnActor()
    {
        //Arrange
        var request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        Actor? addedActor = null;

        //capture the actor passed to the repo
        _actorRepositoryMock
            .Setup(x => x.AddActorAsync(It.IsAny<Actor>()))
            .Callback<Actor>(actor => addedActor = actor)
            .Returns(Task.CompletedTask);

        //simulate the db generating an id when saving
        _actorRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .Callback(() => addedActor!.ActorId = 1)
            .Returns(Task.CompletedTask);

        //Act
        var response = await _actorService.AddActorAsync(request);

        //Assert

        //check if the repo AddActorAsync was called once
        //with the data from the request
        _actorRepositoryMock.Verify(
            x => x.AddActorAsync(It.Is<Actor> (actor =>
            actor.Name == request.Name &&
            actor.BirthDate == request.BirthDate &&
            actor.ImageUrl == request.ImageUrl)),
            Times.Once);

        //check if the repo SaveChangesAsync() was called once
        _actorRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        //check the returned response
        Assert.Equal(1, response.ActorId);
        Assert.Equal(request.Name, response.Name);
        Assert.Equal(request.BirthDate, response.BirthDate);
        Assert.Equal(request.ImageUrl, response.ImageUrl);
    }

    //_____________________GET AN ACTOR BY ID TESTS
    [Fact]
    public async Task GetActorById_WhenIdExist()
    {
        //Arrange
        int actorId = 1;

        Actor actor = new Actor
        {
            ActorId = actorId,
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        //suppose the retrieved actor is actor
        _actorRepositoryMock
            .Setup(x => x.GetActorAsync(actorId))
            .ReturnsAsync(actor);

        //Act
        var result = await _actorService.GetActorByIdAsync(actorId);

        //Assert

        //check if the GetActorAsync was called once
        _actorRepositoryMock.Verify(
            x => x.GetActorAsync(actorId),
            Times.Once);

        //check the response
        Assert.Equal(actor.Name, result?.Name);
        Assert.Equal(actor.BirthDate, result?.BirthDate);
        Assert.Equal(actor.ImageUrl, result?.ImageUrl);
    }

    [Fact]
    public async Task GetActorById_WhenIdDoesNotExist()
    {
        //Arrange
        int actorId = 1;

        Actor? actor = null;

        //suppose the retrieved actor is actor
        _actorRepositoryMock
            .Setup(x => x.GetActorAsync(actorId))
            .ReturnsAsync(actor);

        //Act
        var result = await _actorService.GetActorByIdAsync(actorId);

        //Assert

        //check if the GetActorAsync was called once
        _actorRepositoryMock.Verify(
            x => x.GetActorAsync(actorId),
            Times.Once);

        //check the response
        Assert.Null(result);
    }

    //_____________________GET ALL ACTORS TEST
    [Fact]
    public async Task GetAllActors()
    {
        //Arrange
        Actor actor1 = new Actor
        {
            ActorId = 1,
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        Actor actor2 = new Actor
        {
            ActorId = 2,
            Name = "Brad",
            BirthDate = new DateOnly(1997, 11, 12),
            ImageUrl = "https://image_example.com/"
        };

        List<Actor> actors = new List<Actor>
        {
            actor1, actor2
        };

        //suppose the retrieved list of actors is actors
        _actorRepositoryMock
            .Setup(x => x.GetAllActorsAsync())
            .ReturnsAsync(actors);

        //Act
        var result = await _actorService.GetAllActorsAsync();

        //Assert

        //check if the GetAllActorsAsync was called once
        _actorRepositoryMock.Verify(
            x => x.GetAllActorsAsync(),
            Times.Once);

        //check the response
        //there should be 2 actors in the result
        Assert.Equal(2, result.Count);

        Assert.Equal(actors[0].ActorId, result[0].ActorId);
        Assert.Equal(actors[0].Name, result[0].Name);
        Assert.Equal(actors[0].BirthDate, result[0].BirthDate);
        Assert.Equal(actors[0].ImageUrl, result[0].ImageUrl);

        Assert.Equal(actors[1].ActorId, result[1].ActorId);
        Assert.Equal(actors[1].Name, result[1].Name);
        Assert.Equal(actors[1].BirthDate, result[1].BirthDate);
        Assert.Equal(actors[1].ImageUrl, result[1].ImageUrl);
    }

    //_____________________DELETE AN ACTOR TESTS
    [Fact]
    public async Task DeleteActor_WhenIdExist()
    {
        //Arrange
        int actorId = 1;

        //suppose the actor to be deleted exists
        //=> repo returns true
        _actorRepositoryMock
            .Setup(x => x.DeleteActorAsync(actorId))
            .ReturnsAsync(true);

        //Act
        var result = await _actorService.DeleteActorAsync(actorId);

        //Assert

        //check if the DeleteActorAsync was called once
        _actorRepositoryMock.Verify(
            x => x.DeleteActorAsync(actorId),
            Times.Once);

        //check if the SaveChangesAsync was called once
        _actorRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        //check the response
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteActor_WhenIdDoesNotExist()
    {
        //Arrange
        int actorId = 1;

        //suppose the actor to be deleted doesn't exist
        //=> repo returns false
        _actorRepositoryMock
            .Setup(x => x.DeleteActorAsync(actorId))
            .ReturnsAsync(false);

        //Act
        var result = await _actorService.DeleteActorAsync(actorId);

        //Assert

        //check if the DeleteActorAsync was called once
        _actorRepositoryMock.Verify(
            x => x.DeleteActorAsync(actorId),
            Times.Once);

        //check if the SaveChangesAsync was never called 
        _actorRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        //check the response
        Assert.False(result);
    }

    //_____________________UPDATE AN ACTOR TESTS
    [Fact]
    public async Task UpdateActor_WhenIdExist()
    {
        //Arrange
        int actorId = 1;

        var request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        Actor? updatedActor = null;

        //simulated that UpdateActorAsync updates the 
        //actor accordingly to the request
        _actorRepositoryMock
            .Setup(x => x.UpdateActorAsync(actorId, It.IsAny<Actor>()))
            .Callback<int, Actor>((id, actor) => updatedActor = actor)
            .ReturnsAsync(true);

        _actorRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        //Act
        var result = await _actorService.UpdateActorAsync(actorId, request);

        //Assert

        //check if the UpdateActorAsync was called once
        //with the proper parameter
        _actorRepositoryMock.Verify(
            x => x.UpdateActorAsync(actorId, 
            It.Is<Actor>(actor => 
            actor.Name == request.Name &&
            actor.BirthDate == request.BirthDate &&
            actor.ImageUrl == request.ImageUrl)),
            Times.Once);

        //check if the SaveChangesAsync was called once
        _actorRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

        //check the response
        Assert.True(result);

        //check the updatedActor
        Assert.Equal(request.Name, updatedActor?.Name);
        Assert.Equal(request.BirthDate, updatedActor?.BirthDate);
        Assert.Equal(request.ImageUrl, updatedActor?.ImageUrl);
    }

    [Fact]
    public async Task UpdateActor_WhenIdDoesNotExist()
    {
        //Arrange
        int actorId = 1;

        var request = new ActorRequest
        {
            Name = "Angelina",
            BirthDate = new DateOnly(1999, 12, 20),
            ImageUrl = "https://image_example.com/"
        };

        //suppose the actor doesn't exist
        _actorRepositoryMock
            .Setup(x => x.UpdateActorAsync(actorId, It.IsAny<Actor>()))
            .ReturnsAsync(false);

        //Act
        var result = await _actorService.UpdateActorAsync(actorId, request);

        //Assert

        //check if the UpdateActorAsync was called once
        //with the proper parameter
        _actorRepositoryMock.Verify(
            x => x.UpdateActorAsync(actorId,
            It.IsAny<Actor>()),
            Times.Once);

        //check if the SaveChangesAsync was never called
        _actorRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

        //check the response
        Assert.False(result);
    }
}
