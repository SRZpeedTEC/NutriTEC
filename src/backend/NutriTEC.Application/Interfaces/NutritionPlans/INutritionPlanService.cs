using NutriTEC.Application.DTOs.NutritionPlans;

namespace NutriTEC.Application.Interfaces.NutritionPlans;

public interface INutritionPlanService
{
    Task<NutritionPlanMutationResponse> CreateAsync(CreateNutritionPlanRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<NutritionPlanSummaryResponse>> GetByNutritionistAsync(int nutritionistCode, CancellationToken cancellationToken);

    Task<NutritionPlanDetailResponse> GetDetailAsync(int planId, CancellationToken cancellationToken);

    Task<NutritionPlanMutationResponse> UpdateAsync(int planId, UpdateNutritionPlanRequest request, CancellationToken cancellationToken);

    Task<NutritionPlanMutationResponse> DeleteAsync(int planId, int nutritionistCode, CancellationToken cancellationToken);

    Task<PlanAssignmentMutationResponse> AssignToClientAsync(int planId, AssignPlanRequest request, CancellationToken cancellationToken);

    Task<PlanAssignmentMutationResponse> CancelAssignmentAsync(int assignmentId, int nutritionistCode, CancellationToken cancellationToken);
}
