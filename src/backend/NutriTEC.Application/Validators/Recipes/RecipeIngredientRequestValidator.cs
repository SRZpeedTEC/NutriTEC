using FluentValidation;
using NutriTEC.Application.DTOs.Recipes;

namespace NutriTEC.Application.Validators.Recipes;

public class RecipeIngredientRequestValidator : AbstractValidator<RecipeIngredientRequest>
{
    public RecipeIngredientRequestValidator()
    {
        // Ingredient validation mirrors barcode and quantity constraints from recipe_product.
        RuleFor(ingredient => ingredient.ProductCode)
            .NotEmpty()
            .WithMessage("El codigo del producto es obligatorio.")
            .Must(code => !string.IsNullOrWhiteSpace(code))
            .WithMessage("El codigo del producto es obligatorio.")
            .MaximumLength(40)
            .WithMessage("El codigo del producto no puede superar los 40 caracteres.");

        RuleFor(ingredient => ingredient.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad del producto debe ser mayor que 0.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("La cantidad del producto supera el valor permitido.")
            .Must(quantity => decimal.Round(quantity, 2) == quantity)
            .WithMessage("La cantidad del producto puede tener como maximo 2 decimales.");
    }
}
