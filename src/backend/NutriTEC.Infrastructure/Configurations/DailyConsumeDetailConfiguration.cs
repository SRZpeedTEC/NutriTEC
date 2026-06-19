using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class DailyConsumeDetailConfiguration : IEntityTypeConfiguration<DailyConsumeDetail>
{
    public void Configure(EntityTypeBuilder<DailyConsumeDetail> entity)
    {
        // The view is keyless because it is a read-only projection of several consumption tables.
        entity.HasNoKey();
        entity.ToView("vw_daily_consume_detail", "dbo");

        entity.Property(detail => detail.ClientId).HasColumnName("client_id");
        entity.Property(detail => detail.ConsumeDate).HasColumnName("consume_date").HasColumnType("date");
        entity.Property(detail => detail.MealTimeId).HasColumnName("meal_time_id");
        entity.Property(detail => detail.MealType).HasColumnName("meal_type").HasMaxLength(20).IsUnicode(false);
        entity.Property(detail => detail.BarCode).HasColumnName("bar_code").HasMaxLength(40).IsUnicode(false);
        entity.Property(detail => detail.ProductName).HasColumnName("product_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(detail => detail.PortionUnit).HasColumnName("portion_unit").HasMaxLength(30).IsUnicode(false);
        entity.Property(detail => detail.PortionSize).HasColumnName("portion_size").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Quantity).HasColumnName("quantity").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Calories).HasColumnName("calories").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Protein).HasColumnName("protein").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Carbohydrates).HasColumnName("carbohydrates").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Fat).HasColumnName("fat").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Sodium).HasColumnName("sodium").HasColumnType("numeric(10, 2)");
    }
}
