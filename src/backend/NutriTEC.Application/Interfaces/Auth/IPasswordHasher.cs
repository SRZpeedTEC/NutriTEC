namespace NutriTEC.Application.Interfaces.Auth;

public interface IPasswordHasher
{
    // Hashing is abstracted so authentication workflows never store or compare plain text passwords directly.
    string HashPassword(string password);
}
