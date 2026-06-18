namespace NutriTEC.Application.DTOs.Auth;

public class LoginResponse
{
    // Login returns safe session data without exposing password hashes or internal authentication details.
    public int UserId { get; set; }

    public int ClientId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public ActivePlanSummaryResponse? ActivePlan { get; set; }

    public string Message { get; set; } = string.Empty;
}
