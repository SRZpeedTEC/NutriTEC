using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class RecipeProductDetailConfiguration : IEntityTypeConfiguration<RecipeProductDetail>
{
    public void Configure(EntityTypeBuilder<RecipeProductDetail> entity)
    {
        // The recipe detail view is keyless and read-only within the EF model.
        entity.HasNoKey();
        entity.ToView("vw_recipe_product_detail", "dbo");

        entity.Property(detail => detail.RecipeId).HasColumnName("recipe_id");
        entity.Property(detail => detail.RecipeName).HasColumnName("recipe_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(detail => detail.TotalCalories).HasColumnName("total_calories").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.ClientId).HasColumnName("client_id");
        entity.Property(detail => detail.BarCode).HasColumnName("bar_code").HasMaxLength(40).IsUnicode(false);
        entity.Property(detail => detail.ProductName).HasColumnName("product_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(detail => detail.PortionUnit).HasColumnName("portion_unit").HasMaxLength(30).IsUnicode(false);
        entity.Property(detail => detail.PortionSize).HasColumnName("portion_size").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.Quantity).HasColumnName("quantity").HasColumnType("numeric(10, 2)");
        entity.Property(detail => detail.CalculatedCalories).HasColumnName("calculated_calories").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedFat).HasColumnName("calculated_fat").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedSodium).HasColumnName("calculated_sodium").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedCarbohydrates).HasColumnName("calculated_carbohydrates").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedProtein).HasColumnName("calculated_protein").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedVitamins).HasColumnName("calculated_vitamins").HasMaxLength(120).IsUnicode(false);
        entity.Property(detail => detail.CalculatedCalcium).HasColumnName("calculated_calcium").HasColumnType("numeric(20, 2)");
        entity.Property(detail => detail.CalculatedIron).HasColumnName("calculated_iron").HasColumnType("numeric(20, 2)");
    }
}
