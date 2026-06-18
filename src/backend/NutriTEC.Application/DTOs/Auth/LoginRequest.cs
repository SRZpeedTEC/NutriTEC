namespace NutriTEC.Application.DTOs.Auth;

public class LoginRequest
{
    // Login accepts only credentials; account type is inferred from related profile tables.
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
