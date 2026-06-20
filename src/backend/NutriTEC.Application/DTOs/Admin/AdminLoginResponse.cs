using NutriTEC.Application.DTOs.Auth;

namespace NutriTEC.Application.DTOs.Admin;

public class AdminLoginResponse
{
    // Admin login mirrors the existing login response shape while exposing the admin profile id.
    public int UserId { get; set; }

    public int AdminId { get; set; }

    public int Age { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public ActivePlanSummaryResponse? ActivePlan { get; set; }

    public string Message { get; set; } = string.Empty;
}
