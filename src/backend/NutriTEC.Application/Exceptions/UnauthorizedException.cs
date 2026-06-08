namespace NutriTEC.Application.Exceptions;

public class UnauthorizedException : NutriTecApplicationException
{
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
