using FluentValidation;
using NutriTEC.Application.DTOs.NutritionPlans;

namespace NutriTEC.Application.Validators.NutritionPlans;

public class AssignPlanRequestValidator : AbstractValidator<AssignPlanRequest>
{
    public AssignPlanRequestValidator()
    {
        RuleFor(r => r.ClientId)
            .GreaterThan(0).WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(r => r.NutritionistCode)
            .GreaterThan(0).WithMessage("El codigo del nutricionista debe ser mayor que 0.");

        RuleFor(r => r.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(r => r.EndDate)
            .Must((request, endDate) => endDate == null || endDate >= request.StartDate)
            .WithMessage("La fecha de fin debe ser mayor o igual a la fecha de inicio.");
    }
}
