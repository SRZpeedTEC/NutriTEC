namespace NutriTEC.Application.Exceptions;

public class ApplicationValidationException : NutriTecApplicationException
{
    public ApplicationValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Se encontraron uno o mas errores de validacion.")
    {
        Errors = errors;
    }

    // Validation errors keep the field-to-message shape produced by FluentValidation.
    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
