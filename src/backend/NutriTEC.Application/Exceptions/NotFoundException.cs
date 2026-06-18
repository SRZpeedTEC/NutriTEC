namespace NutriTEC.Application.Exceptions;

public class NotFoundException : NutriTecApplicationException
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}
