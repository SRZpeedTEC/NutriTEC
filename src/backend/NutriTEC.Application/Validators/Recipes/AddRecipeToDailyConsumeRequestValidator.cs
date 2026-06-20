using FluentValidation;
using NutriTEC.Application.DTOs.Recipes;

namespace NutriTEC.Application.Validators.Recipes;

public class AddRecipeToDailyConsumeRequestValidator : AbstractValidator<AddRecipeToDailyConsumeRequest>
{
    public AddRecipeToDailyConsumeRequestValidator()
    {
        // Daily expansion validates ownership context, selected meal type, and multiplier precision.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.MealTimeId)
            .GreaterThan(0)
            .WithMessage("El identificador del horario de comida debe ser mayor que 0.");

        RuleFor(request => request.Multiplier)
            .GreaterThan(0)
            .WithMessage("El multiplicador debe ser mayor que 0.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("El multiplicador supera el valor permitido.")
            .Must(multiplier => decimal.Round(multiplier, 2) == multiplier)
            .WithMessage("El multiplicador puede tener como maximo 2 decimales.");
    }
}
