using FluentValidation;
using NutriTEC.Application.DTOs.Admin;

namespace NutriTEC.Application.Validators.Admin;

public class AdminBillingReportRequestValidator : AbstractValidator<AdminBillingReportRequest>
{
    private static readonly string[] AllowedFrequencies = ["WEEKLY", "MONTHLY", "ANNUAL"];
    private static readonly IReadOnlyDictionary<string, decimal> ExpectedPricesByFrequency =
        new Dictionary<string, decimal>
        {
            ["WEEKLY"] = 1m,
            ["MONTHLY"] = 4m,
            ["ANNUAL"] = 52m
        };

    public AdminBillingReportRequestValidator()
    {
        RuleFor(request => request.Frequency)
            .Must(value => string.IsNullOrWhiteSpace(value)
                || AllowedFrequencies.Contains(value.Trim().ToUpperInvariant()))
            .WithMessage("La frecuencia debe ser WEEKLY, MONTHLY o ANNUAL.");

        RuleFor(request => request.CycleStartDate)
            .NotEmpty()
            .WithMessage("La fecha inicial del ciclo es obligatoria.");

        RuleFor(request => request.CycleEndDate)
            .NotEmpty()
            .WithMessage("La fecha final del ciclo es obligatoria.");

        RuleFor(request => request.PricePerPatient)
            .GreaterThan(0)
            .WithMessage("El precio del ciclo por paciente debe ser mayor que 0.")
            .When(request => request.PricePerPatient.HasValue);

        RuleFor(request => request)
            .Must(request => !request.PricePerPatient.HasValue || !string.IsNullOrWhiteSpace(request.Frequency))
            .WithMessage("La frecuencia es obligatoria cuando se envia un precio por paciente.");

        RuleFor(request => request)
            .Must(PriceMatchesFrequency)
            .WithMessage("El precio por paciente debe coincidir con la frecuencia: WEEKLY=1, MONTHLY=4, ANNUAL=52.")
            .When(request => request.PricePerPatient.HasValue
                && !string.IsNullOrWhiteSpace(request.Frequency)
                && AllowedFrequencies.Contains(request.Frequency.Trim().ToUpperInvariant()));

        RuleFor(request => request)
            .Must(request => request.CycleStartDate.Date <= request.CycleEndDate.Date)
            .WithMessage("La fecha inicial del ciclo debe ser menor o igual que la fecha final.")
            .When(request => request.CycleStartDate != default && request.CycleEndDate != default);

        RuleFor(request => request)
            .Must(request => request.Frequency!.Trim().ToUpperInvariant() != "ANNUAL"
                || request.CycleStartDate.Date.Year == request.CycleEndDate.Date.Year)
            .WithMessage("Un ciclo anual debe estar dentro del mismo anio calendario.")
            .When(request => request.CycleStartDate != default
                && request.CycleEndDate != default
                && !string.IsNullOrWhiteSpace(request.Frequency));
    }

    private static bool PriceMatchesFrequency(AdminBillingReportRequest request)
    {
        var frequency = request.Frequency!.Trim().ToUpperInvariant();
        var price = Math.Round(request.PricePerPatient!.Value, 2, MidpointRounding.AwayFromZero);
        return ExpectedPricesByFrequency[frequency] == price;
    }
}
