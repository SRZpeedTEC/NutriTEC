using NutriTEC.Application.DTOs.NutritionPlans;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.NutritionPlans;
using NutriTEC.Application.Interfaces.Nutritionists;
using NutriTEC.Application.Interfaces.Products;
using NutriTEC.Domain.Entities;
using NutriTEC.Domain.Enums;

namespace NutriTEC.Application.Services;

public class NutritionPlanService : INutritionPlanService
{
    private readonly INutritionPlanRepository _planRepository;
    private readonly INutritionistRepository _nutritionistRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IProductRepository _productRepository;

    public NutritionPlanService(
        INutritionPlanRepository planRepository,
        INutritionistRepository nutritionistRepository,
        IClientRepository clientRepository,
        IProductRepository productRepository)
    {
        _planRepository = planRepository;
        _nutritionistRepository = nutritionistRepository;
        _clientRepository = clientRepository;
        _productRepository = productRepository;
    }

    public async Task<NutritionPlanMutationResponse> CreateAsync(
        CreateNutritionPlanRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureNutritionistExistsAsync(request.NutritionistCode, cancellationToken);

        var mealTimes = await BuildMealTimesAsync(request.MealTimes, cancellationToken);
        var totalCalories = ComputeTotalCalories(mealTimes);
        EnsureCaloriesFitSchema(totalCalories);

        // Each plan creates its own meal_time rows linked via plan_meal_time.
        var plan = new NutritionPlan
        {
            PlanName = request.PlanName.Trim(),
            NutritionistCode = request.NutritionistCode,
            TotalCalories = totalCalories,
            PlanMealTimes = mealTimes.Select(mt => new PlanMealTime { MealTime = mt }).ToList()
        };

        await _planRepository.AddAsync(plan, cancellationToken);

        var created = await _planRepository.GetByIdAsync(plan.PlanId, cancellationToken)
            ?? throw new NotFoundException("Error al recuperar el plan creado.");

        return new NutritionPlanMutationResponse
        {
            Message = "Plan nutricional creado correctamente.",
            Plan = BuildDetailResponse(created)
        };
    }

    public async Task<IReadOnlyCollection<NutritionPlanSummaryResponse>> GetByNutritionistAsync(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        await EnsureNutritionistExistsAsync(nutritionistCode, cancellationToken);

        var plans = await _planRepository.GetByNutritionistCodeAsync(nutritionistCode, cancellationToken);

        return plans.Select(p => new NutritionPlanSummaryResponse
        {
            PlanId = p.PlanId,
            PlanName = p.PlanName,
            TotalCalories = p.TotalCalories,
            NutritionistCode = p.NutritionistCode,
            MealTimeCount = p.PlanMealTimes.Count
        }).ToList();
    }

    public async Task<NutritionPlanDetailResponse> GetDetailAsync(
        int planId,
        CancellationToken cancellationToken)
    {
        var plan = await GetExistingPlanAsync(planId, cancellationToken);
        return BuildDetailResponse(plan);
    }

    public async Task<NutritionPlanMutationResponse> UpdateAsync(
        int planId,
        UpdateNutritionPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await GetExistingPlanAsync(planId, cancellationToken);

        if (plan.NutritionistCode != request.NutritionistCode)
        {
            throw new UnauthorizedException("El plan no pertenece al nutricionista indicado.");
        }

        var newMealTimes = await BuildMealTimesAsync(request.MealTimes, cancellationToken);
        var totalCalories = ComputeTotalCalories(newMealTimes);
        EnsureCaloriesFitSchema(totalCalories);

        plan.PlanName = request.PlanName.Trim();
        plan.TotalCalories = totalCalories;

        await _planRepository.ReplaceMealTimesAsync(plan, newMealTimes, cancellationToken);

        var updated = await _planRepository.GetByIdAsync(planId, cancellationToken)
            ?? throw new NotFoundException("Error al recuperar el plan actualizado.");

        return new NutritionPlanMutationResponse
        {
            Message = "Plan nutricional actualizado correctamente.",
            Plan = BuildDetailResponse(updated)
        };
    }

    public async Task<NutritionPlanMutationResponse> DeleteAsync(
        int planId,
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        var plan = await GetExistingPlanAsync(planId, cancellationToken);

        if (plan.NutritionistCode != nutritionistCode)
        {
            throw new UnauthorizedException("El plan no pertenece al nutricionista indicado.");
        }

        if (plan.PlanAssignments.Any(a => a.AssignmentStatus == "ACTIVE"))
        {
            throw new ConflictException("No se puede eliminar un plan con asignaciones activas.");
        }

        var snapshot = BuildDetailResponse(plan);
        await _planRepository.DeleteAsync(plan, cancellationToken);

        return new NutritionPlanMutationResponse
        {
            Message = "Plan nutricional eliminado correctamente.",
            Plan = snapshot
        };
    }

    public async Task<PlanAssignmentMutationResponse> AssignToClientAsync(
        int planId,
        AssignPlanRequest request,
        CancellationToken cancellationToken)
    {
        var plan = await GetExistingPlanAsync(planId, cancellationToken);

        if (plan.NutritionistCode != request.NutritionistCode)
        {
            throw new UnauthorizedException("El plan no pertenece al nutricionista indicado.");
        }

        if (!await _clientRepository.ExistsByIdAsync(request.ClientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }

        var isActivePatient = await _nutritionistRepository.GetActivePatientAsync(
            request.NutritionistCode, request.ClientId, cancellationToken);

        if (isActivePatient is null)
        {
            throw new ConflictException("El cliente no esta asociado como paciente activo de este nutricionista.");
        }

        var assignment = new PlanAssignment
        {
            PlanId = planId,
            ClientId = request.ClientId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AssignmentStatus = "ACTIVE"
        };

        await _planRepository.AddAssignmentAsync(assignment, cancellationToken);

        return new PlanAssignmentMutationResponse
        {
            AssignmentId = assignment.AssignmentId,
            PlanId = planId,
            ClientId = request.ClientId,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            AssignmentStatus = assignment.AssignmentStatus,
            Message = "Plan asignado al paciente correctamente."
        };
    }

    public async Task<PlanAssignmentMutationResponse> CancelAssignmentAsync(
        int assignmentId,
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        var assignment = await _planRepository.GetAssignmentByIdAsync(assignmentId, cancellationToken);

        if (assignment is null)
        {
            throw new NotFoundException("No se encontro la asignacion solicitada.");
        }

        if (assignment.NutritionPlan.NutritionistCode != nutritionistCode)
        {
            throw new UnauthorizedException("La asignacion no pertenece al nutricionista indicado.");
        }

        await _planRepository.DeleteAssignmentAsync(assignment, cancellationToken);

        return new PlanAssignmentMutationResponse
        {
            AssignmentId = assignmentId,
            PlanId = assignment.PlanId,
            ClientId = assignment.ClientId,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            AssignmentStatus = assignment.AssignmentStatus,
            Message = "Asignacion de plan cancelada correctamente."
        };
    }

    public async Task<ClientActivePlanDetailResponse?> GetActiveByClientAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var (assignment, plan) = await _planRepository.GetActiveByClientAsync(clientId, today, cancellationToken);

        if (assignment is null || plan is null) return null;

        return new ClientActivePlanDetailResponse
        {
            AssignmentId = assignment.AssignmentId,
            PlanId = plan.PlanId,
            PlanName = plan.PlanName,
            TotalCalories = plan.TotalCalories,
            NutritionistCode = plan.NutritionistCode,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            MealTimes = plan.PlanMealTimes.Select(pmt => new NutritionPlanMealTimeResponse
            {
                MealTimeId = pmt.MealTimeId,
                MealType = pmt.MealTime.MealType,
                TotalCalories = pmt.MealTime.Products.Sum(p => p.Calories * p.Quantity),
                Products = pmt.MealTime.Products.Select(p => new NutritionPlanMealTimeProductResponse
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.Product?.ProductName ?? string.Empty,
                    Quantity = p.Quantity,
                    Calories = p.Calories
                }).ToList()
            }).ToList()
        };
    }

    private async Task<NutritionPlan> GetExistingPlanAsync(int planId, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);

        if (plan is null)
        {
            throw new NotFoundException("No se encontro el plan nutricional solicitado.");
        }

        return plan;
    }

    private async Task<List<MealTime>> BuildMealTimesAsync(
        List<PlanMealTimeRequest> mealTimeRequests,
        CancellationToken cancellationToken)
    {
        var allCodes = mealTimeRequests
            .SelectMany(mt => mt.Products.Select(p => p.ProductCode.Trim()))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var products = await _productRepository.GetByBarCodesAsync(allCodes, cancellationToken);

        if (products.Count != allCodes.Count)
        {
            throw new NotFoundException("Uno o mas productos del plan no fueron encontrados.");
        }

        if (products.Any(p => p.ProductStatus != ProductStatus.Active))
        {
            throw new ConflictException("Todos los productos del plan deben estar aprobados.");
        }

        var productMap = products.ToDictionary(p => p.BarCode, StringComparer.OrdinalIgnoreCase);

        return mealTimeRequests.Select(mt => new MealTime
        {
            MealType = mt.MealType.Trim().ToUpperInvariant(),
            Products = mt.Products.Select(p =>
            {
                var product = productMap[p.ProductCode.Trim()];
                return new MealTimeProduct
                {
                    ProductCode = p.ProductCode.Trim(),
                    Quantity = p.Quantity,
                    Calories = product.Calories
                };
            }).ToList()
        }).ToList();
    }

    private static decimal ComputeTotalCalories(IEnumerable<MealTime> mealTimes)
    {
        return mealTimes.Sum(mt => mt.Products.Sum(p => p.Calories * p.Quantity));
    }

    private static NutritionPlanDetailResponse BuildDetailResponse(NutritionPlan plan)
    {
        return new NutritionPlanDetailResponse
        {
            PlanId = plan.PlanId,
            PlanName = plan.PlanName,
            TotalCalories = plan.TotalCalories,
            NutritionistCode = plan.NutritionistCode,
            MealTimes = plan.PlanMealTimes.Select(pmt => new NutritionPlanMealTimeResponse
            {
                MealTimeId = pmt.MealTimeId,
                MealType = pmt.MealTime.MealType,
                TotalCalories = pmt.MealTime.Products.Sum(p => p.Calories * p.Quantity),
                Products = pmt.MealTime.Products.Select(p => new NutritionPlanMealTimeProductResponse
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.Product?.ProductName ?? string.Empty,
                    Quantity = p.Quantity,
                    Calories = p.Calories
                }).ToList()
            }).ToList()
        };
    }

    private static void EnsureCaloriesFitSchema(decimal totalCalories)
    {
        if (totalCalories > 99999999.99m)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(CreateNutritionPlanRequest.MealTimes)] =
                    ["El total de calorias del plan supera el valor permitido."]
            });
        }
    }

    private async Task EnsureNutritionistExistsAsync(int nutritionistCode, CancellationToken cancellationToken)
    {
        if (!await _nutritionistRepository.ExistsByCodeAsync(nutritionistCode, cancellationToken))
        {
            throw new NotFoundException("No se encontro el nutricionista solicitado.");
        }
    }
}
