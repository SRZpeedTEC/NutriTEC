using Microsoft.EntityFrameworkCore;
using NutriTEC.Application.Interfaces.Users;
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
}
