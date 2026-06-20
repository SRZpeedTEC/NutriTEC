using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> entity)
    {
        // Recipe persistence mirrors the client ownership and trigger-maintained total in SQL Server.
        entity.ToTable("recipe");
        entity.HasKey(recipe => recipe.RecipeId).HasName("pk_recipe");

        entity.Property(recipe => recipe.RecipeId).HasColumnName("recipe_id");
        entity.Property(recipe => recipe.RecipeName).HasColumnName("recipe_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(recipe => recipe.TotalCalories).HasColumnName("total_calories").HasColumnType("numeric(10, 2)");
        entity.Property(recipe => recipe.ClientId).HasColumnName("client_id");

        entity.HasOne(recipe => recipe.Client)
            .WithMany()
            .HasForeignKey(recipe => recipe.ClientId)
            .HasConstraintName("fk_recipe_client")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
