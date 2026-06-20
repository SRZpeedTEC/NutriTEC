using NutriTEC.Application.DTOs.Nutritionist;

namespace NutriTEC.Application.Interfaces.Nutritionists;

public interface INutritionistService
{
    Task<IReadOnlyCollection<SearchClientResponse>> SearchClientsAsync(string query, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PatientSummaryResponse>> GetPatientsAsync(int nutritionistCode, CancellationToken cancellationToken);

    Task<PatientAssociationResponse> AssociatePatientAsync(int nutritionistCode, int clientId, CancellationToken cancellationToken);

    Task<PatientAssociationResponse> DisassociatePatientAsync(int nutritionistCode, int clientId, CancellationToken cancellationToken);
}
