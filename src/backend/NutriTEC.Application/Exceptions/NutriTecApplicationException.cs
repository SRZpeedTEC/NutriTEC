namespace NutriTEC.Application.Exceptions;

public abstract class NutriTecApplicationException : Exception
{
    protected NutriTecApplicationException(string message)
        : base(message)
    {
    }
}
