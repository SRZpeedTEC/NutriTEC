using FluentValidation;
using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        // Login validation checks only request shape before the service evaluates credentials.
        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("El correo electronico es obligatorio.")
            .EmailAddress()
            .WithMessage("El correo electronico no tiene un formato valido.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("La contrasena es obligatoria.");
    }
}
