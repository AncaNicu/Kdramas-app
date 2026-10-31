using KdramasApp_BE.DTOs;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Interfaces;

namespace KdramasApp_BE.Services.Classes;

public class ActorService: IActorService
{
    private readonly IActorRepository _repository;

    public ActorService(IActorRepository repository)
    {
        this._repository = repository;
    }

    public ActorResponse MapToActorResponse(Actor actor)
    {
        ActorResponse response = new ActorResponse
        {
            ActorId = actor.ActorId,
            Name = actor.Name,
            BirthDate = actor.BirthDate,
            ImageUrl = actor.ImageUrl
        };

        return response;
    }

    public async Task<ActorResponse> AddActorAsync(ActorRequest request)
    {
        Actor newActor = new Actor
        {
            Name = request.Name,
            BirthDate = request.BirthDate,
            ImageUrl = request.ImageUrl
        };

        //call the repo to add the new actor
        await _repository.AddActorAsync(newActor);

        //saves the changes
        await _repository.SaveChangesAsync();

        return this.MapToActorResponse(newActor);
    }

    public async Task<ActorResponse?> GetActorByIdAsync(int actorId)
    {
        Actor? actor = await _repository.GetActorAsync(actorId);
        
        if(actor == null)
        {
            return null;
        }

        ActorResponse response = this.MapToActorResponse(actor);

        return response;
    }

    public async Task<List<ActorResponse>> GetAllActorsAsync()
    {
        List<Actor> actors = await _repository.GetAllActorsAsync();

        List<ActorResponse> response = new List<ActorResponse>();

        foreach(Actor actor in actors)
        {
            response.Add(this.MapToActorResponse(actor));
        }
        
        return response;
    }

    public async Task<bool> DeleteActorAsync(int actorId)
    {
        //call the repo to delete the actor
        var deleteActor = await _repository.DeleteActorAsync(actorId);

        if(deleteActor)
        {
            //saves the changes
            await _repository.SaveChangesAsync();
        }
        return deleteActor;
    }

    public async Task<bool> UpdateActorAsync(int actorId, ActorRequest request)
    {
        Actor newActor = new Actor
        {
            Name = request.Name,
            BirthDate = request.BirthDate,
            ImageUrl = request.ImageUrl
        };

        //call the repo to update the actor
        var updateActor = await _repository.UpdateActorAsync(actorId, newActor);

        if(updateActor)
        {
            //saves the changes
            await _repository.SaveChangesAsync();
        }

        return updateActor;
    }
}
