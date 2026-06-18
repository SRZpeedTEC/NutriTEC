namespace NutriTEC.Application.Exceptions;

public class ConflictException : NutriTecApplicationException
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
