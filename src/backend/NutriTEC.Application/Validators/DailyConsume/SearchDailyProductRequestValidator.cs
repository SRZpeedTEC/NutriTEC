using FluentValidation;
using NutriTEC.Application.DTOs.DailyConsume;

namespace NutriTEC.Application.Validators.DailyConsume;

public class SearchDailyProductRequestValidator : AbstractValidator<SearchDailyProductRequest>
{
    public SearchDailyProductRequestValidator()
    {
        // Search validation avoids unbounded approved-product queries.
        RuleFor(request => request.Query)
            .NotEmpty()
            .WithMessage("El texto de busqueda es obligatorio.")
            .Must(query => !string.IsNullOrWhiteSpace(query))
            .WithMessage("El texto de busqueda es obligatorio.")
            .MaximumLength(120)
            .WithMessage("El texto de busqueda no puede superar los 120 caracteres.");
    }
}
