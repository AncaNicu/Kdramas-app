using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
namespace KdramasApp_BE.Data;

public class KdramasDbContext : DbContext
{
    public KdramasDbContext(DbContextOptions<KdramasDbContext> options) : base(options)
    {

    }
    public DbSet<Kdrama> Kdramas { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<KdramaActor> KdramaActors { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserKdrama> UserKdramas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //searches all the implementations of IEntityTypeConfiguration<T>
        //and applies them
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KdramasDbContext).Assembly);
    }
}
