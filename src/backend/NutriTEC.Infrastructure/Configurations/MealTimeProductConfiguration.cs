using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class MealTimeProductConfiguration : IEntityTypeConfiguration<MealTimeProduct>
{
    public void Configure(EntityTypeBuilder<MealTimeProduct> entity)
    {
        // Product details retain the existing meal-time/product composite identity.
        entity.ToTable("meal_time_product", table =>
            table.HasTrigger("trg_update_daily_consume_totals"));
        entity.HasKey(detail => new { detail.MealTimeId, detail.ProductCode }).HasName("pk_meal_time_product");

        entity.Property(detail => detail.MealTimeId).HasColumnName("meal_time_id");
        entity.Property(detail => detail.ProductCode).HasColumnName("product_code").HasMaxLength(40).IsUnicode(false);
        entity.Property(detail => detail.Calories).HasColumnName("calories").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Quantity).HasColumnName("quantity").HasColumnType("numeric(10, 2)");

        entity.HasOne(detail => detail.MealTime)
            .WithMany(mealTime => mealTime.Products)
            .HasForeignKey(detail => detail.MealTimeId)
            .HasConstraintName("fk_meal_time_product_meal_time")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(detail => detail.Product)
            .WithMany()
            .HasForeignKey(detail => detail.ProductCode)
            .HasConstraintName("fk_meal_time_product_product")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
