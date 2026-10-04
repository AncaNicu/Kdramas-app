using KdramasApp_BE.Enums;
using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KdramasApp_BE.Data.Configurations;

public class KdramaActorConfiguration : IEntityTypeConfiguration<KdramaActor>
{
    public void Configure(EntityTypeBuilder<KdramaActor> builder)
    {
        builder.HasKey(x => new
        {
            x.KdramaId, x.ActorId
        });

        builder.Property(x => x.RoleType)
            .IsRequired()
            .HasConversion<string>();

        //FKs
        builder.HasOne(x => x.Kdrama)
            .WithMany(x => x.KdramaActors)
            .HasForeignKey(x => x.KdramaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Actor)
            .WithMany(x => x.KdramaActors)
            .HasForeignKey(x => x.ActorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
