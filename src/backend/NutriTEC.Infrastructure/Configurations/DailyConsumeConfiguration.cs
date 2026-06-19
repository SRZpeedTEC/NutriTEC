using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class DailyConsumeConfiguration : IEntityTypeConfiguration<DailyConsume>
{
    public void Configure(EntityTypeBuilder<DailyConsume> entity)
    {
        // Daily summaries use the same client/date composite key as the SQL Server table.
        entity.ToTable("daily_consume");
        entity.HasKey(consume => new { consume.ClientId, consume.ConsumeDate }).HasName("pk_daily_consume");

        entity.Property(consume => consume.ClientId).HasColumnName("client_id");
        entity.Property(consume => consume.ConsumeDate).HasColumnName("consume_date").HasColumnType("date");
        entity.Property(consume => consume.TotalCalories).HasColumnName("total_calories").HasColumnType("numeric(10, 2)");

        entity.HasOne(consume => consume.Client)
            .WithMany()
            .HasForeignKey(consume => consume.ClientId)
            .HasConstraintName("fk_daily_consume_client")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
