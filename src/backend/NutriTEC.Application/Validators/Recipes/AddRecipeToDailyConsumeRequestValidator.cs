using FluentValidation;
using NutriTEC.Application.DTOs.Recipes;

namespace NutriTEC.Application.Validators.Recipes;

public class AddRecipeToDailyConsumeRequestValidator : AbstractValidator<AddRecipeToDailyConsumeRequest>
{
    private static readonly string[] ValidMealTypes =
        ["BREAKFAST", "LUNCH", "DINNER", "SNACK", "OTHER"];

    public AddRecipeToDailyConsumeRequestValidator()
    {
        // Daily expansion validates ownership context, selected meal type, and multiplier precision.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.MealType)
            .NotEmpty().WithMessage("El tipo de comida es obligatorio.")
            .Must(type => ValidMealTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            .WithMessage("El tipo de comida debe ser: BREAKFAST, LUNCH, DINNER, SNACK u OTHER.");

        RuleFor(request => request.Multiplier)
            .GreaterThan(0)
            .WithMessage("El multiplicador debe ser mayor que 0.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("El multiplicador supera el valor permitido.")
            .Must(multiplier => decimal.Round(multiplier, 2) == multiplier)
            .WithMessage("El multiplicador puede tener como maximo 2 decimales.");
    }
}
