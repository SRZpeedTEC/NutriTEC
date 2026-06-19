using FluentValidation;
using NutriTEC.Application.DTOs.DailyConsume;

namespace NutriTEC.Application.Validators.DailyConsume;

public class DeleteDailyProductRequestValidator : AbstractValidator<DeleteDailyProductRequest>
{
    public DeleteDailyProductRequestValidator()
    {
        // Delete validation checks every component needed to locate and authorize the composite detail.
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
    }
}
