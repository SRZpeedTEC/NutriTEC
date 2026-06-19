using FluentValidation;
using NutriTEC.Application.DTOs.DailyConsume;

namespace NutriTEC.Application.Validators.DailyConsume;

public class AddDailyProductRequestValidator : AbstractValidator<AddDailyProductRequest>
{
    public AddDailyProductRequestValidator()
    {
        // Add validation mirrors the existing foreign keys, barcode length, and positive quantity check.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.MealTimeId)
            .GreaterThan(0)
            .WithMessage("El identificador del horario de comida debe ser mayor que 0.");

        RuleFor(request => request.ProductCode)
            .NotEmpty()
            .WithMessage("El codigo del producto es obligatorio.")
            .Must(code => !string.IsNullOrWhiteSpace(code))
            .WithMessage("El codigo del producto es obligatorio.")
            .MaximumLength(40)
            .WithMessage("El codigo del producto no puede superar los 40 caracteres.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad consumida debe ser mayor que 0.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("La cantidad consumida supera el valor permitido.")
            .Must(quantity => decimal.Round(quantity, 2) == quantity)
            .WithMessage("La cantidad consumida puede tener como maximo 2 decimales.");
    }
}
