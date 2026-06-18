namespace NutriTEC.Application.Interfaces.Auth;

public interface IPasswordHasher
{
    // Hashing is abstracted so authentication workflows never store or compare plain text passwords directly.
    string HashPassword(string password);

    // Verification stays behind the same abstraction so login does not depend on a concrete hashing library.
    bool VerifyPassword(string password, string passwordHash);
}
