using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.NutritionPlans;

public interface INutritionPlanRepository
{
    Task AddAsync(NutritionPlan plan, CancellationToken cancellationToken);

    Task<NutritionPlan?> GetByIdAsync(int planId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<NutritionPlan>> GetByNutritionistCodeAsync(int nutritionistCode, CancellationToken cancellationToken);

    Task ReplaceMealTimesAsync(NutritionPlan plan, IEnumerable<MealTime> newMealTimes, CancellationToken cancellationToken);

    Task DeleteAsync(NutritionPlan plan, CancellationToken cancellationToken);

    Task AddAssignmentAsync(PlanAssignment assignment, CancellationToken cancellationToken);

    Task<PlanAssignment?> GetAssignmentByIdAsync(int assignmentId, CancellationToken cancellationToken);

    Task DeleteAssignmentAsync(PlanAssignment assignment, CancellationToken cancellationToken);
}
