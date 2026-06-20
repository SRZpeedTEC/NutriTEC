using Microsoft.EntityFrameworkCore;
using NutriTEC.Application.Interfaces.NutritionPlans;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class NutritionPlanRepository : INutritionPlanRepository
{
    private readonly NutriTecDbContext _dbContext;

    public NutritionPlanRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(NutritionPlan plan, CancellationToken cancellationToken)
    {
        await _dbContext.NutritionPlans.AddAsync(plan, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<NutritionPlan?> GetByIdAsync(int planId, CancellationToken cancellationToken)
    {
        return _dbContext.NutritionPlans
            .Include(p => p.PlanMealTimes)
                .ThenInclude(pmt => pmt.MealTime)
                    .ThenInclude(mt => mt.Products)
            .Include(p => p.PlanAssignments)
            .FirstOrDefaultAsync(p => p.PlanId == planId, cancellationToken);
    }

    public Task<IReadOnlyCollection<NutritionPlan>> GetByNutritionistCodeAsync(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        return _dbContext.NutritionPlans
            .AsNoTracking()
            .Include(p => p.PlanMealTimes)
            .Where(p => p.NutritionistCode == nutritionistCode)
            .OrderBy(p => p.PlanName)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyCollection<NutritionPlan>)t.Result, cancellationToken);
    }

    public async Task ReplaceMealTimesAsync(
        NutritionPlan plan,
        IEnumerable<MealTime> newMealTimes,
        CancellationToken cancellationToken)
    {
        // Remove only the plan_meal_time links; keep orphaned meal_time rows
        // since they may still be referenced by daily_meal_time records.
        _dbContext.PlanMealTimes.RemoveRange(plan.PlanMealTimes);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var newLinks = newMealTimes.Select(mt => new PlanMealTime
        {
            PlanId = plan.PlanId,
            MealTime = mt
        }).ToList();

        await _dbContext.PlanMealTimes.AddRangeAsync(newLinks, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(NutritionPlan plan, CancellationToken cancellationToken)
    {
        _dbContext.NutritionPlans.Remove(plan);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAssignmentAsync(PlanAssignment assignment, CancellationToken cancellationToken)
    {
        await _dbContext.PlanAssignments.AddAsync(assignment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PlanAssignment?> GetAssignmentByIdAsync(int assignmentId, CancellationToken cancellationToken)
    {
        return _dbContext.PlanAssignments
            .Include(a => a.NutritionPlan)
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId, cancellationToken);
    }

    public async Task DeleteAssignmentAsync(PlanAssignment assignment, CancellationToken cancellationToken)
    {
        _dbContext.PlanAssignments.Remove(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
