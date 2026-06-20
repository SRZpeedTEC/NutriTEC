using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class PlanMealTimeConfiguration : IEntityTypeConfiguration<PlanMealTime>
{
    public void Configure(EntityTypeBuilder<PlanMealTime> entity)
    {
        entity.ToTable("plan_meal_time");
        entity.HasKey(pmt => pmt.PlanMealTimeId).HasName("pk_plan_meal_time");

        entity.Property(pmt => pmt.PlanMealTimeId).HasColumnName("plan_meal_time_id");
        entity.Property(pmt => pmt.MealTimeId).HasColumnName("meal_time_id");
        entity.Property(pmt => pmt.PlanId).HasColumnName("plan_id");

        entity.HasOne(pmt => pmt.Plan)
            .WithMany(p => p.PlanMealTimes)
            .HasForeignKey(pmt => pmt.PlanId)
            .HasConstraintName("FK_PLAN_MEAL_TIME_NUTRITION_PLAN")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(pmt => pmt.MealTime)
            .WithMany(mt => mt.PlanMealTimes)
            .HasForeignKey(pmt => pmt.MealTimeId)
            .HasConstraintName("FK_PLAN_MEAL_TIME_MEAL_TIME")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasIndex(pmt => new { pmt.PlanId, pmt.MealTimeId })
            .IsUnique()
            .HasDatabaseName("uq_plan_meal_time_plan_meal");
    }
}
