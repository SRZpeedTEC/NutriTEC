using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class MealTimeConfiguration : IEntityTypeConfiguration<MealTime>
{
    public void Configure(EntityTypeBuilder<MealTime> entity)
    {
        // The meal-time identity scopes the products used by either plans or daily consumption.
        entity.ToTable("meal_time");
        entity.HasKey(mealTime => mealTime.MealTimeId).HasName("pk_meal_time");

        entity.Property(mealTime => mealTime.MealTimeId).HasColumnName("meal_time_id");
        entity.Property(mealTime => mealTime.MealType).HasColumnName("meal_type").HasMaxLength(20).IsUnicode(false);
    }
}
