using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Interfaces.Nutritionists;

public interface INutritionistRepository
{
    Task<bool> ExistsByCodeAsync(int nutritionistCode, CancellationToken cancellationToken);

    Task<bool> IdNumberExistsAsync(string idNumber, CancellationToken cancellationToken);

    Task RegisterNutritionistAsync(User user, Nutritionist nutritionist, CancellationToken cancellationToken);

    Task<Nutritionist?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<NutritionistClient>> GetActivePatientsAsync(int nutritionistCode, CancellationToken cancellationToken);

    Task<NutritionistClient?> GetActivePatientAsync(int nutritionistCode, int clientId, CancellationToken cancellationToken);

    Task AssociatePatientAsync(NutritionistClient association, CancellationToken cancellationToken);

    Task DisassociatePatientAsync(NutritionistClient association, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<(int ClientId, int UserId, string FullName, string Email, int Age, string Country)>> SearchClientsAsync(
        string query,
        CancellationToken cancellationToken);
}
