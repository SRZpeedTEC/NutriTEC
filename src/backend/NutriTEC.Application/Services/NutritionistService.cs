using NutriTEC.Application.DTOs.Nutritionist;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.Nutritionists;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Services;

public class NutritionistService : INutritionistService
{
    private readonly INutritionistRepository _nutritionistRepository;
    private readonly IClientRepository _clientRepository;

    public NutritionistService(
        INutritionistRepository nutritionistRepository,
        IClientRepository clientRepository)
    {
        _nutritionistRepository = nutritionistRepository;
        _clientRepository = clientRepository;
    }

    public async Task<IReadOnlyCollection<SearchClientResponse>> SearchClientsAsync(
        string query,
        CancellationToken cancellationToken)
    {
        var results = await _nutritionistRepository.SearchClientsAsync(query.Trim(), cancellationToken);

        return results.Select(r => new SearchClientResponse
        {
            ClientId = r.ClientId,
            UserId = r.UserId,
            FullName = r.FullName,
            Email = r.Email,
            Age = r.Age,
            Country = r.Country
        }).ToList();
    }

    public async Task<IReadOnlyCollection<PatientSummaryResponse>> GetPatientsAsync(
        int nutritionistCode,
        CancellationToken cancellationToken)
    {
        await EnsureNutritionistExistsAsync(nutritionistCode, cancellationToken);

        var patients = await _nutritionistRepository.GetActivePatientsAsync(nutritionistCode, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.Today);

        return patients.Select(nc =>
        {
            var hasActivePlan = nc.Client.PlanAssignments.Any(a =>
                a.AssignmentStatus == "ACTIVE"
                && a.StartDate <= today
                && (a.EndDate == null || a.EndDate >= today));

            return new PatientSummaryResponse
            {
                ClientId = nc.ClientId,
                FullName = $"{nc.Client.User.Name} {nc.Client.User.LastName}",
                Email = nc.Client.User.Email,
                Age = nc.Client.User.Age,
                Country = nc.Client.Country,
                AssociationDate = nc.StartDate,
                HasActivePlan = hasActivePlan
            };
        }).ToList();
    }

    public async Task<PatientAssociationResponse> AssociatePatientAsync(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        await EnsureNutritionistExistsAsync(nutritionistCode, cancellationToken);

        if (!await _clientRepository.ExistsByIdAsync(clientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }

        var existing = await _nutritionistRepository.GetActivePatientAsync(nutritionistCode, clientId, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("El cliente ya esta asociado como paciente de este nutricionista.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var association = new NutritionistClient
        {
            NutritionistCode = nutritionistCode,
            ClientId = clientId,
            StartDate = today,
            Status = "ACTIVE"
        };

        await _nutritionistRepository.AssociatePatientAsync(association, cancellationToken);

        return new PatientAssociationResponse
        {
            NutritionistCode = nutritionistCode,
            ClientId = clientId,
            AssociationDate = today,
            Message = "Paciente asociado correctamente."
        };
    }

    public async Task<PatientAssociationResponse> DisassociatePatientAsync(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        await EnsureNutritionistExistsAsync(nutritionistCode, cancellationToken);

        var association = await _nutritionistRepository.GetActivePatientAsync(nutritionistCode, clientId, cancellationToken);
        if (association is null)
        {
            throw new NotFoundException("El cliente no esta asociado como paciente activo de este nutricionista.");
        }

        await _nutritionistRepository.DisassociatePatientAsync(association, cancellationToken);

        return new PatientAssociationResponse
        {
            NutritionistCode = nutritionistCode,
            ClientId = clientId,
            AssociationDate = association.StartDate,
            Message = "Paciente desasociado correctamente."
        };
    }

    private async Task EnsureNutritionistExistsAsync(int nutritionistCode, CancellationToken cancellationToken)
    {
        if (!await _nutritionistRepository.ExistsByCodeAsync(nutritionistCode, cancellationToken))
        {
            throw new NotFoundException("No se encontro el nutricionista solicitado.");
        }
    }
}
