using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class RecipeProductConfiguration : IEntityTypeConfiguration<RecipeProduct>
{
    public void Configure(EntityTypeBuilder<RecipeProduct> entity)
    {
        // EF is informed about the trigger so SQL Server-compatible DML is generated for ingredient changes.
        entity.ToTable("recipe_product", table =>
            table.HasTrigger("trg_update_recipe_nutrition_totals"));
        entity.HasKey(ingredient => new { ingredient.RecipeId, ingredient.ProductCode })
            .HasName("pk_recipe_product");

        entity.Property(ingredient => ingredient.RecipeId).HasColumnName("recipe_id");
        entity.Property(ingredient => ingredient.ProductCode).HasColumnName("product_code").HasMaxLength(40).IsUnicode(false);
        entity.Property(ingredient => ingredient.Quantity).HasColumnName("quantity").HasColumnType("numeric(10, 2)");

        entity.HasOne(ingredient => ingredient.Recipe)
            .WithMany(recipe => recipe.Products)
            .HasForeignKey(ingredient => ingredient.RecipeId)
            .HasConstraintName("fk_recipe_product_recipe")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(ingredient => ingredient.Product)
            .WithMany()
            .HasForeignKey(ingredient => ingredient.ProductCode)
            .HasConstraintName("fk_recipe_product_product")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
