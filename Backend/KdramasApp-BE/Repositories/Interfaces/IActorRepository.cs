using KdramasApp_BE.Models;
namespace KdramasApp_BE.Repositories.Interfaces;

public interface IActorRepository
{
    Task<List<Actor>> GetAllActorsAsync();
    Task AddActorAsync(Actor actor);
    Task<bool> DeleteActorAsync(int actorId);
    Task<bool> UpdateActorAsync(int actorId, Actor newActor);
    Task<Actor?> GetActorAsync(int actorId);
    Task<Actor?> GetActorWithKdramasAsync(int actorId);
    Task SaveChangesAsync();
}
