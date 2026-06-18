using NutriTEC.Application.Interfaces.Auth;

namespace NutriTEC.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // BCrypt.Net-Next handles salt generation and work-factor metadata inside the stored hash.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // Invalid or legacy hash formats should fail closed without leaking details to the login workflow.
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
