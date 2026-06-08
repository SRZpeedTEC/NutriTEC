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
}
