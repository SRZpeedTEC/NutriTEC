namespace NutriTEC.Application.DTOs.Auth;

public class AuthServiceResult<TResponse>
{
    // Services use this result object to report business outcomes without depending on HTTP types.
    public bool Succeeded { get; private init; }

    public bool Conflict { get; private init; }

    public string? ErrorMessage { get; private init; }

    public TResponse? Value { get; private init; }

    public static AuthServiceResult<TResponse> Success(TResponse value)
    {
        return new AuthServiceResult<TResponse>
        {
            Succeeded = true,
            Value = value
        };
    }

    public static AuthServiceResult<TResponse> EmailConflict(string message)
    {
        return new AuthServiceResult<TResponse>
        {
            Conflict = true,
            ErrorMessage = message
        };
    }
}
