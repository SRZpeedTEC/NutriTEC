using FluentValidation;
using NutriTEC.Application.DTOs.Recipes;

namespace NutriTEC.Application.Validators.Recipes;

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
        // Creation requires valid ownership, a meaningful name, and an unambiguous ingredient set.
        RuleFor(request => request.ClientId)
            .GreaterThan(0)
            .WithMessage("El identificador del cliente debe ser mayor que 0.");

        RuleFor(request => request.RecipeName)
            .NotEmpty()
            .WithMessage("El nombre de la receta es obligatorio.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("El nombre de la receta es obligatorio.")
            .MaximumLength(120)
            .WithMessage("El nombre de la receta no puede superar los 120 caracteres.");

        RuleFor(request => request.Products)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("La lista de productos es obligatoria.")
            .NotEmpty()
            .WithMessage("La receta debe contener al menos un producto.")
            .Must(HaveUniqueProducts)
            .WithMessage("La receta no puede contener productos duplicados.");

        RuleForEach(request => request.Products)
            .SetValidator(new RecipeIngredientRequestValidator());
    }

    private static bool HaveUniqueProducts(IEnumerable<RecipeIngredientRequest> products)
    {
        // Barcode comparison follows the database's usual case-insensitive identifier behavior.
        var codes = products.Select(product => product.ProductCode?.Trim() ?? string.Empty).ToList();
        return codes.Distinct(StringComparer.OrdinalIgnoreCase).Count() == codes.Count;
    }
}
