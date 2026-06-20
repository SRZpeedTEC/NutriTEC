using FluentValidation;
using NutriTEC.Application.DTOs.NutritionPlans;

namespace NutriTEC.Application.Validators.NutritionPlans;

public class CreateNutritionPlanRequestValidator : AbstractValidator<CreateNutritionPlanRequest>
{
    private static readonly string[] ValidMealTypes =
        ["BREAKFAST", "LUNCH", "DINNER", "SNACK", "OTHER"];

    public CreateNutritionPlanRequestValidator()
    {
        RuleFor(r => r.PlanName)
            .NotEmpty().WithMessage("El nombre del plan es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre del plan no puede superar los 120 caracteres.");

        RuleFor(r => r.NutritionistCode)
            .GreaterThan(0).WithMessage("El codigo del nutricionista debe ser mayor que 0.");

        RuleFor(r => r.MealTimes)
            .NotEmpty().WithMessage("El plan debe tener al menos un tiempo de comida.")
            .Must(mealTimes => mealTimes.Count <= 5).WithMessage("El plan no puede tener mas de 5 tiempos de comida.")
            .Must(HaveUniqueMealTypes).WithMessage("Los tiempos de comida no pueden repetirse.");

        RuleForEach(r => r.MealTimes).ChildRules(mealTime =>
        {
            mealTime.RuleFor(mt => mt.MealType)
                .NotEmpty().WithMessage("El tipo de comida es obligatorio.")
                .Must(type => ValidMealTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
                .WithMessage("El tipo de comida debe ser: BREAKFAST, LUNCH, DINNER, SNACK u OTHER.");

            mealTime.RuleFor(mt => mt.Products)
                .NotEmpty().WithMessage("Cada tiempo de comida debe tener al menos un producto.");

            mealTime.RuleForEach(mt => mt.Products).ChildRules(product =>
            {
                product.RuleFor(p => p.ProductCode)
                    .NotEmpty().WithMessage("El codigo de producto es obligatorio.");

                product.RuleFor(p => p.Quantity)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0.");
            });
        });
    }

    private static bool HaveUniqueMealTypes(List<PlanMealTimeRequest> mealTimes)
    {
        var types = mealTimes.Select(mt => mt.MealType.Trim().ToUpperInvariant()).ToList();
        return types.Count == types.Distinct().Count();
    }
}
