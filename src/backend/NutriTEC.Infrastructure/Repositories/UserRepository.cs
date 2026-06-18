using Microsoft.EntityFrameworkCore;
using NutriTEC.Application.Interfaces.Users;
using NutriTEC.Domain.Entities;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NutriTecDbContext _dbContext;

    public UserRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        // Email lookups are normalized to match the service's canonical lower-case email value.
        return _dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        // Login requires the user credentials only; account subtype records are queried separately.
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task<bool> ExistsByIdAsync(int userId, CancellationToken cancellationToken)
    {
        // Submitter checks keep product workflows from relying on database constraint failures.
        return _dbContext.Users.AnyAsync(user => user.UserId == userId, cancellationToken);
    }
}
