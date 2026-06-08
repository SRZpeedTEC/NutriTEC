using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> entity)
    {
        // This mapping keeps client profile persistence aligned with the current SQL Server schema.
        entity.ToTable("client");
        entity.HasKey(client => client.ClientId).HasName("pk_client");

        entity.Property(client => client.ClientId).HasColumnName("client_id");
        entity.Property(client => client.MaxDailyCalories).HasColumnName("max_daily_calories").HasColumnType("numeric(10, 2)");
        entity.Property(client => client.Country).HasColumnName("country").HasMaxLength(80).IsUnicode(false);
        entity.Property(client => client.UserId).HasColumnName("user_id");

        entity.HasOne(client => client.User)
            .WithOne(user => user.Client)
            .HasForeignKey<Client>(client => client.UserId)
            .HasConstraintName("fk_client_app_user")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasIndex(client => client.UserId).IsUnique().HasDatabaseName("uq_client_user_id");
    }
}
