using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class DailyMealTimeConfiguration : IEntityTypeConfiguration<DailyMealTime>
{
    public void Configure(EntityTypeBuilder<DailyMealTime> entity)
    {
        // The link retains both client and date so ownership can be checked before mutations.
        entity.ToTable("daily_meal_time", table =>
            table.HasTrigger("trg_validate_daily_meal_time_uniqueness"));
        entity.HasKey(dailyMealTime => dailyMealTime.DailyMealTimeId).HasName("pk_daily_meal_time");

        entity.Property(dailyMealTime => dailyMealTime.DailyMealTimeId).HasColumnName("daily_meal_time_id");
        entity.Property(dailyMealTime => dailyMealTime.ClientId).HasColumnName("client_id");
        entity.Property(dailyMealTime => dailyMealTime.ConsumeDate).HasColumnName("consume_date").HasColumnType("date");
        entity.Property(dailyMealTime => dailyMealTime.MealTimeId).HasColumnName("meal_time_id");

        entity.HasOne(dailyMealTime => dailyMealTime.DailyConsume)
            .WithMany(consume => consume.MealTimes)
            .HasForeignKey(dailyMealTime => new { dailyMealTime.ClientId, dailyMealTime.ConsumeDate })
            .HasConstraintName("fk_daily_meal_time_daily_consume")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(dailyMealTime => dailyMealTime.MealTime)
            .WithMany(mealTime => mealTime.DailyMealTimes)
            .HasForeignKey(dailyMealTime => dailyMealTime.MealTimeId)
            .HasConstraintName("fk_daily_meal_time_meal_time")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
