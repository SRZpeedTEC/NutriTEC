namespace NutriTEC.Domain.Entities;

public class User
{
    // The SQL schema keeps common authentication and identity fields in app_user.
    public int UserId { get; set; }

    public DateOnly Birthday { get; set; }

    public int Age
    {
        get
        {
            // Age remains derived from birthday so it cannot become stale in persistent storage.
            if (Birthday == default)
            {
                return 0;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - Birthday.Year;
            return Birthday > today.AddYears(-age) ? age - 1 : age;
        }
    }

    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string HashPassword { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Client? Client { get; set; }

    public Nutritionist? Nutritionist { get; set; }
}
