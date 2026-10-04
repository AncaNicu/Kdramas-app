using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KdramasApp_BE.Data.Configurations;

public class UserKdramaConfiguration : IEntityTypeConfiguration<UserKdrama>
{
    public void Configure(EntityTypeBuilder<UserKdrama> builder)
    {
        //Composed PK
        builder.HasKey(x => new
        {
            x.UserId, x.KdramaId
        });

        builder.Property(x => x.WatchPlatform)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.WatchedYear)
            .IsRequired();

        builder.Property(x => x.WatchedMonth)
            .IsRequired()
            .HasConversion<string>();


        //FKs
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserKdramas)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Kdrama)
            .WithMany(x => x.UserKdramas)
            .HasForeignKey(x => x.KdramaId)
            .OnDelete(DeleteBehavior.Cascade);


        //Check constraints
        builder.ToTable(table =>
            table.HasCheckConstraint(
                "CK_Valid_WatchedYear",
                "[WatchedYear] >= 1900 AND [WatchedYear] <= 2050"
             ));
    }
}
