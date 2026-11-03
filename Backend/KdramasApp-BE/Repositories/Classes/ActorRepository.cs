using KdramasApp_BE.Data;
using KdramasApp_BE.Models;
using KdramasApp_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace KdramasApp_BE.Repositories.Classes;

public class ActorRepository : IActorRepository
{
    private readonly KdramasDbContext _context;
    public ActorRepository(KdramasDbContext context)
    {
        this._context = context;
    }
    public async Task AddActorAsync(Actor actor)
    {
        await _context.Actors.AddAsync(actor);
    }

    public async Task<bool> DeleteActorAsync(int actorId)
    {
        var actor = await _context.Actors.FindAsync(actorId);
        if(actor != null)
        {
            _context.Actors.Remove(actor);
            return true;
        }
        return false;
    }

    public async Task<Actor?> GetActorAsync(int actorId)
    {
        var actor = await _context.Actors.FindAsync(actorId);
        return actor;
    }

    public async Task<List<Actor>> GetAllActorsAsync()
    {
        var actors = await _context.Actors.ToListAsync();
        return actors;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateActorAsync(int actorId, Actor newActor)
    {
        var actor = await _context.Actors.FindAsync(actorId);
        if(actor != null)
        {
            actor.Name = newActor.Name;
            actor.BirthDate = newActor.BirthDate;
            actor.ImageUrl = newActor.ImageUrl;
            return true;
        }
        return false;
    }
    public async Task<Actor?> GetActorWithKdramasAsync(int actorId)
    {
        return await _context.Actors
            .AsNoTracking()//tells that we only read the data (we don't modify it)
            .Include(actor => actor.KdramaActors)
            .ThenInclude(kdramaActor => kdramaActor.Kdrama)
            .FirstOrDefaultAsync(actor => actor.ActorId == actorId);
    }
}
