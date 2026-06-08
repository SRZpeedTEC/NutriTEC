using NutriTEC.Application.Interfaces.Auth;

namespace NutriTEC.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // BCrypt.Net-Next handles salt generation and work-factor metadata inside the stored hash.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
