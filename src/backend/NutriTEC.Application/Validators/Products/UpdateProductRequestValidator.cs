using FluentValidation;
using NutriTEC.Application.DTOs.Products;

namespace NutriTEC.Application.Validators.Products;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        // Update validation excludes barcode because route identity cannot be changed.
        RuleFor(request => request.ProductName)
            .NotEmpty()
            .WithMessage("El nombre del producto es obligatorio.")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(120)
            .WithMessage("El nombre del producto no puede superar los 120 caracteres.");

        RuleFor(request => request.PortionUnit)
            .NotEmpty()
            .WithMessage("La unidad de porcion es obligatoria.")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("La unidad de porcion es obligatoria.")
            .MaximumLength(30)
            .WithMessage("La unidad de porcion no puede superar los 30 caracteres.");

        // Nutrition values mirror the non-negative and positive checks from the product table.
        RuleFor(request => request.PortionSize)
            .GreaterThan(0)
            .WithMessage("El tamano de porcion debe ser mayor que 0.");

        RuleFor(request => request.Calories)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Las calorias no pueden ser negativas.");

        RuleFor(request => request.Fat)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La grasa no puede ser negativa.");

        RuleFor(request => request.Sodium)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El sodio no puede ser negativo.");

        RuleFor(request => request.Carbohydrates)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Los carbohidratos no pueden ser negativos.");

        RuleFor(request => request.Protein)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La proteina no puede ser negativa.");

        RuleFor(request => request.Vitamins)
            .NotEmpty()
            .WithMessage("Las vitaminas son obligatorias.")
            .Must(value => !string.IsNullOrWhiteSpace(value))
            .WithMessage("Las vitaminas son obligatorias.")
            .MaximumLength(120)
            .WithMessage("Las vitaminas no pueden superar los 120 caracteres.")
            .Must(HaveValidVitaminList)
            .WithMessage("Las vitaminas deben indicarse como nombres alfanumericos separados por coma.");

        RuleFor(request => request.Calcium)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El calcio no puede ser negativo.");

        RuleFor(request => request.Iron)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El hierro no puede ser negativo.");

        RuleFor(request => request.UserId)
            .GreaterThan(0)
            .WithMessage("El identificador del usuario debe ser mayor que 0.");
    }

    private static bool HaveValidVitaminList(string vitamins)
    {
        if (string.IsNullOrWhiteSpace(vitamins))
        {
            return false;
        }

        return vitamins
            .Split(',')
            .Select(vitamin => vitamin.Trim())
            .All(vitamin => vitamin.Length > 0 && vitamin.All(char.IsLetterOrDigit));
    }
}
