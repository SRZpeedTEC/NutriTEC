using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Infrastructure.Configurations;

public class PlanAssignmentConfiguration : IEntityTypeConfiguration<PlanAssignment>
{
    public void Configure(EntityTypeBuilder<PlanAssignment> entity)
    {
        // Plan assignments link clients to plans and provide the active status/date range.
        entity.ToTable("plan_assignment");
        entity.HasKey(assignment => assignment.AssignmentId).HasName("pk_plan_assignment");

        entity.Property(assignment => assignment.AssignmentId).HasColumnName("assignment_id");
        entity.Property(assignment => assignment.StartDate).HasColumnName("start_date").HasColumnType("date");
        entity.Property(assignment => assignment.EndDate).HasColumnName("end_date").HasColumnType("date");
        entity.Property(assignment => assignment.AssignmentStatus).HasColumnName("assignment_status").HasMaxLength(20).IsUnicode(false);
        entity.Property(assignment => assignment.PlanId).HasColumnName("plan_id");
        entity.Property(assignment => assignment.ClientId).HasColumnName("client_id");

        entity.HasOne(assignment => assignment.NutritionPlan)
            .WithMany(plan => plan.PlanAssignments)
            .HasForeignKey(assignment => assignment.PlanId)
            .HasConstraintName("fk_plan_assignment_nutrition_plan")
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(assignment => assignment.Client)
            .WithMany(client => client.PlanAssignments)
            .HasForeignKey(assignment => assignment.ClientId)
            .HasConstraintName("fk_plan_assignment_client")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
