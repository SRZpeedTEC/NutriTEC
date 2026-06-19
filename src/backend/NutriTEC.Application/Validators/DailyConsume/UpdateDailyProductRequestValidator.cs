using FluentValidation;
using NutriTEC.Application.DTOs.DailyConsume;

namespace NutriTEC.Application.Validators.DailyConsume;

public class UpdateDailyProductRequestValidator : AbstractValidator<UpdateDailyProductRequest>
{
    public UpdateDailyProductRequestValidator()
    {
        // Update bodies contain only ownership context and the new positive quantity.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad consumida debe ser mayor que 0.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("La cantidad consumida supera el valor permitido.")
            .Must(quantity => decimal.Round(quantity, 2) == quantity)
            .WithMessage("La cantidad consumida puede tener como maximo 2 decimales.");
    }
}
