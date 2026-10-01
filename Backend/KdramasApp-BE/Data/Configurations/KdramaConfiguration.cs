using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KdramasApp_BE.Data.Configurations;

public class KdramaConfiguration : IEntityTypeConfiguration<Kdrama>
{
    public void Configure(EntityTypeBuilder<Kdrama> builder)
    {

    }
}
