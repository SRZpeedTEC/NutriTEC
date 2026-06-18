using FluentValidation;
using NutriTEC.Application.DTOs.Products;

namespace NutriTEC.Application.Validators.Products;

public class DeleteProductRequestValidator : AbstractValidator<DeleteProductRequest>
{
    public DeleteProductRequestValidator()
    {
        // Delete validation checks route and ownership inputs before service business rules.
        RuleFor(request => request.BarCode)
            .NotEmpty()
            .WithMessage("El codigo de barras es obligatorio.")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("El codigo de barras es obligatorio.")
            .MaximumLength(40)
            .WithMessage("El codigo de barras no puede superar los 40 caracteres.");

        RuleFor(request => request.UserId)
            .GreaterThan(0)
            .WithMessage("El identificador del usuario debe ser mayor que 0.");
    }
}
