using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NutriTEC.Application.Interfaces.Nutritionists;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class NutritionistRepository : INutritionistRepository
{
    private readonly NutriTecDbContext _dbContext;

    public NutritionistRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByCodeAsync(int nutritionistCode, CancellationToken cancellationToken)
    {
        return _dbContext.Nutritionists.AnyAsync(n => n.NutritionistCode == nutritionistCode, cancellationToken);
    }

    public Task<bool> IdNumberExistsAsync(string idNumber, CancellationToken cancellationToken)
    {
        return _dbContext.Nutritionists.AnyAsync(n => n.IdNumber == idNumber, cancellationToken);
    }

    public async Task RegisterNutritionistAsync(
        User user,
        Nutritionist nutritionist,
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        nutritionist.User = user;

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Nutritionists.AddAsync(nutritionist, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task<Nutritionist?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.Nutritionists
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.UserId == userId, cancellationToken);
    }

    public Task<IReadOnlyCollection<NutritionistClient>> GetActivePatientsAsync(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        return _dbContext.NutritionistClients
            .AsNoTracking()
            .Include(nc => nc.Client)
                .ThenInclude(c => c.User)
            .Include(nc => nc.Client)
                .ThenInclude(c => c.PlanAssignments)
            .Where(nc => nc.NutritionistCode == nutritionistCode && nc.Status == "ACTIVE")
            .OrderBy(nc => nc.StartDate)
            .ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyCollection<NutritionistClient>)t.Result, cancellationToken);
    }

    public Task<NutritionistClient?> GetActivePatientAsync(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        return _dbContext.NutritionistClients
            .FirstOrDefaultAsync(
                nc => nc.NutritionistCode == nutritionistCode
                   && nc.ClientId == clientId
                   && nc.Status == "ACTIVE",
                cancellationToken);
    }

    public async Task AssociatePatientAsync(NutritionistClient association, CancellationToken cancellationToken)
    {
        await _dbContext.NutritionistClients.AddAsync(association, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DisassociatePatientAsync(NutritionistClient association, CancellationToken cancellationToken)
    {
        // Preserve history by marking the association as CANCELLED instead of deleting.
        association.Status = "CANCELLED";
        association.EndDate = DateOnly.FromDateTime(DateTime.Today);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<(int ClientId, int UserId, string FullName, string Email, int Age, string Country)>> SearchClientsAsync(
        string query,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.ToLowerInvariant();

        var results = await _dbContext.Clients
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c =>
                c.User.Name.ToLower().Contains(normalizedQuery) ||
                c.User.LastName.ToLower().Contains(normalizedQuery) ||
                c.User.Email.ToLower().Contains(normalizedQuery))
            .OrderBy(c => c.User.LastName)
            .Take(50)
            .Select(c => new
            {
                c.ClientId,
                c.UserId,
                c.User.Name,
                c.User.LastName,
                c.User.Email,
                c.User.Birthday,
                c.Country
            })
            .ToListAsync(cancellationToken);

        return results.Select(r =>
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - r.Birthday.Year;
            if (r.Birthday > today.AddYears(-age)) age--;
            return (r.ClientId, r.UserId, $"{r.Name} {r.LastName}", r.Email, age, r.Country);
        }).ToList();
    }
}
