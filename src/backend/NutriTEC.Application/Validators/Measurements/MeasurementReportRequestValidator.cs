using FluentValidation;
using NutriTEC.Application.DTOs.Measurements;

namespace NutriTEC.Application.Validators.Measurements;

public class MeasurementReportRequestValidator : AbstractValidator<MeasurementReportRequest>
{
    public MeasurementReportRequestValidator()
    {
        // Range validation protects the inclusive query from missing, inverted, or future dates.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("La fecha inicial es obligatoria.")
            .Must(date => date.Date <= DateTime.Today)
            .WithMessage("La fecha inicial no puede ser mayor que la fecha actual.");

        RuleFor(request => request.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("La fecha final es obligatoria.")
            .Must(date => date.Date <= DateTime.Today)
            .WithMessage("La fecha final no puede ser mayor que la fecha actual.");

        RuleFor(request => request)
            .Must(request => request.StartDate.Date <= request.EndDate.Date)
            .WithMessage("La fecha inicial debe ser menor o igual que la fecha final.")
            .When(request => request.StartDate != default && request.EndDate != default);
    }
}
