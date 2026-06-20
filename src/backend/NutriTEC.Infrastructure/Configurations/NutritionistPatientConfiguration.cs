using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class NutritionistClientConfiguration : IEntityTypeConfiguration<NutritionistClient>
{
    public void Configure(EntityTypeBuilder<NutritionistClient> entity)
    {
        entity.ToTable("nutritionist_client");
        entity.HasKey(nc => new { nc.NutritionistCode, nc.ClientId, nc.StartDate }).HasName("pk_nutritionist_client");

        entity.Property(nc => nc.NutritionistCode).HasColumnName("nutritionist_code");
        entity.Property(nc => nc.ClientId).HasColumnName("client_id");
        entity.Property(nc => nc.StartDate).HasColumnName("start_date").HasColumnType("date");
        entity.Property(nc => nc.EndDate).HasColumnName("end_date").HasColumnType("date").IsRequired(false);
        entity.Property(nc => nc.Status).HasColumnName("status").HasMaxLength(20).IsUnicode(false);

        entity.HasOne(nc => nc.Nutritionist)
            .WithMany(n => n.Patients)
            .HasForeignKey(nc => nc.NutritionistCode)
            .HasConstraintName("FK_NUTRITIONIST_CLIENT_NUTRITIONIST")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(nc => nc.Client)
            .WithMany(c => c.NutritionistAssociations)
            .HasForeignKey(nc => nc.ClientId)
            .HasConstraintName("FK_NUTRITIONIST_CLIENT_CLIENT")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
