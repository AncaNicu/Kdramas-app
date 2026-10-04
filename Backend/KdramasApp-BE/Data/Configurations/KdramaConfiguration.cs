using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KdramasApp_BE.Data.Configurations;

public class KdramaConfiguration : IEntityTypeConfiguration<Kdrama>
{
    public void Configure(EntityTypeBuilder<Kdrama> builder)
    {
        builder.HasKey(x => x.KdramaId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ReleaseYear)
            .IsRequired();

        builder.Property(x => x.BroadcastingNetwork)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NoOfEpisodes)
            .IsRequired();

        builder.Property(x => x.PosterUrl)
            .IsRequired(false)
            .HasMaxLength(500);

        //Check constraints
        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_NoEps_Positive",
                "[NoOfEpisodes] > 0"
                );
            table.HasCheckConstraint(
                "CK_Valid_ReleaseYear",
                "[ReleaseYear] >= 1900 AND [ReleaseYear] <= 2050"
             );
        });
    }
}
