using FluentValidation;
using NutriTEC.Application.DTOs.Measurements;

namespace NutriTEC.Application.Validators.Measurements;

public class CreateMeasurementRequestValidator : AbstractValidator<CreateMeasurementRequest>
{
    public CreateMeasurementRequestValidator()
    {
        // Creation validation protects request shape before client and duplicate-date checks run in the service.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.MeasurementDate)
            .NotEmpty()
            .WithMessage("La fecha de medicion es obligatoria.")
            .Must(measurementDate => measurementDate.Date <= DateTime.Today)
            .WithMessage("La fecha de medicion no puede ser mayor que la fecha actual.");

        // Body values follow the same positive and percentage boundaries used by client registration.
        RuleFor(request => request.BodyWeight)
            .GreaterThan(0)
            .WithMessage("El peso corporal debe ser mayor que 0.");

        RuleFor(request => request.BodyMassIndex)
            .GreaterThan(0)
            .WithMessage("El indice de masa corporal debe ser mayor que 0.");

        RuleFor(request => request.Waist)
            .GreaterThan(0)
            .WithMessage("La medida de cintura debe ser mayor que 0.");

        RuleFor(request => request.Neck)
            .GreaterThan(0)
            .WithMessage("La medida de cuello debe ser mayor que 0.");

        RuleFor(request => request.Hip)
            .GreaterThan(0)
            .WithMessage("La medida de cadera debe ser mayor que 0.");

        RuleFor(request => request.MusclePercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("El porcentaje de musculo debe estar entre 0 y 100.");

        RuleFor(request => request.FatPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("El porcentaje de grasa debe estar entre 0 y 100.");

        RuleFor(request => request)
            .Must(request => request.MusclePercentage + request.FatPercentage <= 100)
            .WithMessage("La suma del porcentaje de musculo y grasa debe ser menor o igual a 100.");
    }
}
