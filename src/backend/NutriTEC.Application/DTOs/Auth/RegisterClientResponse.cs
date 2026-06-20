namespace NutriTEC.Application.DTOs.Auth;

public class RegisterClientResponse
{
    // The response confirms the created account without exposing password material.
    public int UserId { get; set; }

    public int ClientId { get; set; }

    public int Age { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
