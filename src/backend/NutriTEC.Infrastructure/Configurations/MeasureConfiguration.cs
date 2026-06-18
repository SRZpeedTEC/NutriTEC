using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class MeasureConfiguration : IEntityTypeConfiguration<Measure>
{
    public void Configure(EntityTypeBuilder<Measure> entity)
    {
        // Measures use the existing composite key because each client can have time-based measurement snapshots.
        entity.ToTable("measure");
        entity.HasKey(measure => new { measure.ClientId, measure.MeasureDateTime }).HasName("pk_measure");

        entity.Property(measure => measure.MeasureDateTime).HasColumnName("measure_datetime").HasColumnType("datetime2");
        entity.Property(measure => measure.Neck).HasColumnName("neck").HasColumnType("numeric(6, 2)");
        entity.Property(measure => measure.MusclePercentage).HasColumnName("muscle_percentage").HasColumnType("numeric(5, 2)");
        entity.Property(measure => measure.BodyWeight).HasColumnName("body_weight").HasColumnType("numeric(6, 2)");
        entity.Property(measure => measure.Hip).HasColumnName("hip").HasColumnType("numeric(6, 2)");
        entity.Property(measure => measure.Waist).HasColumnName("waist").HasColumnType("numeric(6, 2)");
        entity.Property(measure => measure.FatPercentage).HasColumnName("fat_percentage").HasColumnType("numeric(5, 2)");
        entity.Property(measure => measure.BodyMassIndex).HasColumnName("body_mass_index").HasColumnType("numeric(5, 2)");
        entity.Property(measure => measure.ClientId).HasColumnName("client_id");

        entity.HasOne(measure => measure.Client)
            .WithMany(client => client.Measures)
            .HasForeignKey(measure => measure.ClientId)
            .HasConstraintName("fk_measure_client")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
