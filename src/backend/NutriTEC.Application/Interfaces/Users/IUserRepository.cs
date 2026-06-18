using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Users;

public interface IUserRepository
{
    // Email checks stay in the repository because uniqueness depends on persisted user data.
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    // Login loads the persisted user credentials by normalized email.
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    // Product submission needs to confirm that the submitter exists before creating a foreign-key relation.
    Task<bool> ExistsByIdAsync(int userId, CancellationToken cancellationToken);
}
