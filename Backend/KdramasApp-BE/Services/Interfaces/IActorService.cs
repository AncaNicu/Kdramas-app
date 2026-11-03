using KdramasApp_BE.DTOs;

namespace KdramasApp_BE.Services.Interfaces;

public interface IActorService
{
    Task<ActorResponse> AddActorAsync(ActorRequest request);
    Task<ActorDetailsResponse?> GetActorByIdAsync(int actorId);
    Task<List<ActorResponse>> GetAllActorsAsync();
    Task<bool> DeleteActorAsync(int actorId);
    Task<bool> UpdateActorAsync(int actorId, ActorRequest request);
}
