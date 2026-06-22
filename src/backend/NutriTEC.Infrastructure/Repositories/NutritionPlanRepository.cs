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

    public async Task<(PlanAssignment? Assignment, NutritionPlan? Plan)> GetActiveByClientAsync(
        int clientId,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var assignment = await _dbContext.PlanAssignments
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .Where(a => a.AssignmentStatus == "ACTIVE")
            .Where(a => a.StartDate <= today)
            .Where(a => a.EndDate == null || a.EndDate >= today)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (assignment is null) return (null, null);

        var plan = await _dbContext.NutritionPlans
            .AsNoTracking()
            .Include(p => p.PlanMealTimes)
                .ThenInclude(pmt => pmt.MealTime)
                    .ThenInclude(mt => mt.Products)
            .FirstOrDefaultAsync(p => p.PlanId == assignment.PlanId, cancellationToken);

        return (assignment, plan);
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

    public async Task FinishActiveAssignmentsAsync(int clientId, CancellationToken cancellationToken)
    {
        // El esquema solo permite un plan ACTIVE por cliente (indice unico filtrado):
        // se cierra cualquier asignacion activa previa antes de crear la nueva.
        var activeAssignments = await _dbContext.PlanAssignments
            .Where(a => a.ClientId == clientId && a.AssignmentStatus == "ACTIVE")
            .ToListAsync(cancellationToken);

        if (activeAssignments.Count == 0)
        {
            return;
        }

        foreach (var assignment in activeAssignments)
        {
            assignment.AssignmentStatus = "FINISHED";
        }

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
