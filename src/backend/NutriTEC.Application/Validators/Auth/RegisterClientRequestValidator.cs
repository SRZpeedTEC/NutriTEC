using FluentValidation;
using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.Validators.Auth;

public class RegisterClientRequestValidator : AbstractValidator<RegisterClientRequest>
{
    public RegisterClientRequestValidator()
    {
        // These rules protect the registration workflow from incomplete account and profile data.
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(80)
            .WithMessage("El nombre no puede superar los 80 caracteres.");

        RuleFor(request => request.LastName)
            .NotEmpty()
            .WithMessage("El apellido es obligatorio.")
            .MaximumLength(80)
            .WithMessage("El apellido no puede superar los 80 caracteres.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("El correo electronico es obligatorio.")
            .EmailAddress()
            .WithMessage("El correo electronico no tiene un formato valido.")
            .MaximumLength(255)
            .WithMessage("El correo electronico no puede superar los 255 caracteres.");

        RuleFor(request => request.Password)
            .NotEmpty()
            .WithMessage("La contrasena es obligatoria.")
            .MinimumLength(8)
            .WithMessage("La contrasena debe tener al menos 8 caracteres.")
            .MaximumLength(128)
            .WithMessage("La contrasena no puede superar los 128 caracteres.");

        RuleFor(request => request.Birthday)
            .NotEmpty()
            .WithMessage("La fecha de nacimiento es obligatoria.")
            .GreaterThanOrEqualTo(new DateOnly(1900, 1, 1))
            .WithMessage("La fecha de nacimiento no puede ser anterior al 1 de enero de 1900.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de nacimiento no puede ser mayor que la fecha actual.")
            .Must(HasPositiveAge)
            .WithMessage("La fecha de nacimiento debe producir una edad positiva.");

        RuleFor(request => request.Country)
            .NotEmpty()
            .WithMessage("El pais es obligatorio.")
            .MaximumLength(80)
            .WithMessage("El pais no puede superar los 80 caracteres.");

        // Health metrics must be positive or within percentage ranges before any database writes happen.
        RuleFor(request => request.BodyWeight)
            .GreaterThan(0)
            .WithMessage("El peso corporal debe ser mayor que 0.");

        RuleFor(request => request.BodyMassIndex)
            .GreaterThan(0)
            .WithMessage("El indice de masa corporal debe ser mayor que 0.");

        RuleFor(request => request.Waist)
            .GreaterThan(0)
            .WithMessage("La medida de cintura debe ser mayor que 0.");

        RuleFor(request => request.Neck)
            .GreaterThan(0)
            .WithMessage("La medida de cuello debe ser mayor que 0.");

        RuleFor(request => request.Hip)
            .GreaterThan(0)
            .WithMessage("La medida de cadera debe ser mayor que 0.");

        RuleFor(request => request.MusclePercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("El porcentaje de musculo debe estar entre 0 y 100.");

        RuleFor(request => request.FatPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("El porcentaje de grasa debe estar entre 0 y 100.");

        RuleFor(request => request)
            .Must(request => request.MusclePercentage + request.FatPercentage <= 100)
            .WithMessage("La suma del porcentaje de musculo y grasa debe ser menor o igual a 100.");

        RuleFor(request => request.MaxDailyCalories)
            .GreaterThan(0)
            .WithMessage("Las calorias maximas diarias deben ser mayores que 0.");
    }

    private static bool HasPositiveAge(DateOnly birthday)
    {
        // Age is derived from the birth date only for validation and is never persisted as stale data.
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthday.Year;

        if (birthday > today.AddYears(-age))
        {
            age--;
        }

        return age > 0;
    }
}
