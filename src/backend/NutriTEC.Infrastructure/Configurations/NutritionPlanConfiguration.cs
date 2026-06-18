using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class NutritionPlanConfiguration : IEntityTypeConfiguration<NutritionPlan>
{
    public void Configure(EntityTypeBuilder<NutritionPlan> entity)
    {
        // This mapping exposes only the persisted nutrition_plan fields needed by login.
        entity.ToTable("nutrition_plan");
        entity.HasKey(plan => plan.PlanId).HasName("pk_nutrition_plan");

        entity.Property(plan => plan.PlanId).HasColumnName("plan_id");
        entity.Property(plan => plan.PlanName).HasColumnName("plan_name").HasMaxLength(120).IsUnicode(false);
        entity.Property(plan => plan.TotalCalories).HasColumnName("total_calories").HasColumnType("numeric(10, 2)");
        entity.Property(plan => plan.NutritionistCode).HasColumnName("nutritionist_code");
    }
}
