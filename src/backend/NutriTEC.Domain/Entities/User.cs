namespace NutriTEC.Domain.Entities;

public class User
{
    // The SQL schema keeps common authentication and identity fields in app_user.
    public int UserId { get; set; }

    public DateOnly Birthday { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string HashPassword { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Client? Client { get; set; }
}
