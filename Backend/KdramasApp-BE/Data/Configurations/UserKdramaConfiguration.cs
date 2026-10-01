using KdramasApp_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KdramasApp_BE.Data.Configurations;

public class UserKdramaConfiguration : IEntityTypeConfiguration<UserKdrama>
{
    public void Configure(EntityTypeBuilder<UserKdrama> builder)
    {

    }
}
