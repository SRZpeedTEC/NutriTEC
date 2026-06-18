using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Clients;

public interface IClientRepository
{
    // Feature workflows use client existence checks before operating on related records.
    Task<bool> ExistsByIdAsync(int clientId, CancellationToken cancellationToken);

    // The repository persists the user, client profile, and initial measurement as one registration unit.
    Task RegisterClientAsync(
        User user,
        Client client,
        Measure initialMeasure,
        CancellationToken cancellationToken);

    // Login infers client accounts from the user-to-client relationship.
    Task<Client?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

    // The active plan lookup reads assignment status and date range from the database.
    Task<PlanAssignment?> GetActivePlanAssignmentAsync(
        int clientId,
        DateOnly currentDate,
        CancellationToken cancellationToken);
}
