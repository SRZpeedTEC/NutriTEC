using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Clients;

public interface IClientRepository
{
    // The repository persists the user, client profile, and initial measurement as one registration unit.
    Task RegisterClientAsync(
        User user,
        Client client,
        Measure initialMeasure,
        CancellationToken cancellationToken);
}
