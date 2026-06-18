using AutoMapper;
using NutriTEC.Application.DTOs.Measurements;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.Measurements;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Services;

public class MeasurementService : IMeasurementService
{
    private readonly IMeasurementRepository _measurementRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public MeasurementService(
        IMeasurementRepository measurementRepository,
        IClientRepository clientRepository,
        IMapper mapper)
    {
        _measurementRepository = measurementRepository;
        _clientRepository = clientRepository;
        _mapper = mapper;
    }

    public async Task<MeasurementMutationResponse> CreateMeasurementAsync(
        CreateMeasurementRequest request,
        CancellationToken cancellationToken)
    {
        // New measurements are stored as one snapshot per client per calendar day.
        request.MeasurementDate = request.MeasurementDate.Date;

        if (!await _clientRepository.ExistsByIdAsync(request.ClientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }

        await ValidateMeasurementDateIsAllowedAsync(
            request.ClientId,
            request.MeasurementDate,
            cancellationToken);

        if (await _measurementRepository.MeasurementDateExistsAsync(
            request.ClientId,
            request.MeasurementDate,
            cancellationToken))
        {
            throw new ConflictException("El cliente ya tiene una medicion registrada para esa fecha.");
        }

        var measure = _mapper.Map<Measure>(request);
        measure.MeasureDateTime = request.MeasurementDate;

        await _measurementRepository.AddAsync(measure, cancellationToken);
        await _measurementRepository.SaveChangesAsync(cancellationToken);

        return CreateMutationResponse("Medicion registrada correctamente.", measure);
    }

    public async Task<MeasurementMutationResponse> UpdateLatestMeasurementAsync(
        int clientId,
        DateTime measurementDate,
        UpdateMeasurementRequest request,
        CancellationToken cancellationToken)
    {
        // Editing is intentionally restricted to the most recent persisted snapshot for the client.
        var normalizedMeasurementDate = measurementDate.Date;

        if (!await _clientRepository.ExistsByIdAsync(clientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }

        ValidateMeasurementDateIsNotFuture(normalizedMeasurementDate);

        var measure = await _measurementRepository.GetByClientIdAndDateAsync(
            clientId,
            normalizedMeasurementDate,
            cancellationToken);

        if (measure is null)
        {
            throw new NotFoundException("No se encontro la medicion solicitada.");
        }

        var latestMeasure = await _measurementRepository.GetLatestByClientIdAsync(clientId, cancellationToken);

        if (latestMeasure is null || latestMeasure.MeasureDateTime.Date != normalizedMeasurementDate)
        {
            throw new ConflictException("Solo se puede editar la medicion mas reciente del cliente.");
        }

        _mapper.Map(request, measure);
        await _measurementRepository.SaveChangesAsync(cancellationToken);

        return CreateMutationResponse("Medicion actualizada correctamente.", measure);
    }

    public async Task<IReadOnlyCollection<MeasurementResponse>> GetClientHistoryAsync(
        int clientId,
        CancellationToken cancellationToken)
    {
        // History is returned only for existing clients and is ordered by the persisted measurement date.
        if (!await _clientRepository.ExistsByIdAsync(clientId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el cliente solicitado.");
        }

        var measures = await _measurementRepository.GetByClientIdAsync(clientId, cancellationToken);
        return measures.Select(measure => _mapper.Map<MeasurementResponse>(measure)).ToList();
    }

    private async Task ValidateMeasurementDateIsAllowedAsync(
        int clientId,
        DateTime measurementDate,
        CancellationToken cancellationToken)
    {
        // The first persisted measurement is the cleanest available registration-date proxy in the current schema.
        ValidateMeasurementDateIsNotFuture(measurementDate);

        var firstMeasure = await _measurementRepository.GetFirstByClientIdAsync(clientId, cancellationToken);

        if (firstMeasure is not null && measurementDate.Date < firstMeasure.MeasureDateTime.Date)
        {
            throw CreateValidationException(
                nameof(CreateMeasurementRequest.MeasurementDate),
                "La fecha de medicion no puede ser anterior a la fecha de registro del cliente.");
        }
    }

    private static void ValidateMeasurementDateIsNotFuture(DateTime measurementDate)
    {
        // Route dates are validated in the service because they do not pass through a FluentValidation request DTO.
        if (measurementDate.Date > DateTime.Today)
        {
            throw CreateValidationException(
                nameof(CreateMeasurementRequest.MeasurementDate),
                "La fecha de medicion no puede ser mayor que la fecha actual.");
        }
    }

    private MeasurementMutationResponse CreateMutationResponse(string message, Measure measure)
    {
        // Both create and update return the same response shape for frontend consistency.
        return new MeasurementMutationResponse
        {
            Message = message,
            Measurement = _mapper.Map<MeasurementResponse>(measure)
        };
    }

    private static ApplicationValidationException CreateValidationException(
        string propertyName,
        string message)
    {
        // Service-level validation uses the same field-error structure as controller validators.
        return new ApplicationValidationException(new Dictionary<string, string[]>
        {
            [propertyName] = new[] { message }
        });
    }
}
