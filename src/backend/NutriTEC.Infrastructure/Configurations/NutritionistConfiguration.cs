using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class NutritionistConfiguration : IEntityTypeConfiguration<Nutritionist>
{
    public void Configure(EntityTypeBuilder<Nutritionist> entity)
    {
        entity.ToTable("nutritionist");
        entity.HasKey(n => n.NutritionistCode).HasName("pk_nutritionist");

        entity.Property(n => n.NutritionistCode).HasColumnName("nutritionist_code");
        entity.Property(n => n.PaymentMethod).HasColumnName("payment_method").HasMaxLength(20).IsUnicode(false);
        entity.Property(n => n.Photo).HasColumnName("photo").HasMaxLength(255).IsUnicode(false).IsRequired(false);
        entity.Property(n => n.Address).HasColumnName("address").HasMaxLength(255).IsUnicode(false);
        entity.Property(n => n.IdNumber).HasColumnName("id_number").HasMaxLength(40).IsUnicode(false);
        entity.Property(n => n.EncryptedCreditCard).HasColumnName("encrypted_credit_card").HasMaxLength(255).IsUnicode(false).IsRequired(false);
        entity.Property(n => n.Weight).HasColumnName("weight").HasColumnType("numeric(6, 2)");
        entity.Property(n => n.BodyMassIndex).HasColumnName("body_mass_index").HasColumnType("numeric(5, 2)");
        entity.Property(n => n.UserId).HasColumnName("user_id");

        entity.HasOne(n => n.User)
            .WithOne(u => u.Nutritionist)
            .HasForeignKey<Nutritionist>(n => n.UserId)
            .HasConstraintName("FK_NUTRITIONIST_APP_USER")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasIndex(n => n.UserId).IsUnique().HasDatabaseName("uq_nutritionist_user_id");
        entity.HasIndex(n => n.IdNumber).IsUnique().HasDatabaseName("uq_nutritionist_id_number");
    }
}
