using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly NutriTecDbContext _dbContext;

    public ClientRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegisterClientAsync(
        User user,
        Client client,
        Measure initialMeasure,
        CancellationToken cancellationToken)
    {
        // A transaction ensures user, client, and first measurement rows are committed together.
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        client.User = user;
        initialMeasure.Client = client;

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.Clients.AddAsync(client, cancellationToken);
        await _dbContext.Measures.AddAsync(initialMeasure, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task<Client?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        // A client row proves the user is supported by the current login flow.
        return _dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(client => client.UserId == userId, cancellationToken);
    }

    public Task<PlanAssignment?> GetActivePlanAssignmentAsync(
        int clientId,
        DateOnly currentDate,
        CancellationToken cancellationToken)
    {
        // Active plan selection follows the assignment status and date range defined in the SQL schema.
        return _dbContext.PlanAssignments
            .AsNoTracking()
            .Include(assignment => assignment.NutritionPlan)
            .Where(assignment => assignment.ClientId == clientId)
            .Where(assignment => assignment.AssignmentStatus == "ACTIVE")
            .Where(assignment => assignment.StartDate <= currentDate)
            .Where(assignment => assignment.EndDate == null || assignment.EndDate >= currentDate)
            .OrderByDescending(assignment => assignment.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
