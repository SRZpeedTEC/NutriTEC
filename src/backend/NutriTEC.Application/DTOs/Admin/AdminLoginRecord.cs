namespace NutriTEC.Application.DTOs.Admin;

public class AdminLoginRecord
{
    // Internal login data includes the password hash so the service can verify credentials in C#.
    public int AdminId { get; set; }

    public int UserId { get; set; }

    public DateOnly Birthday { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string HashPassword { get; set; } = string.Empty;
}
