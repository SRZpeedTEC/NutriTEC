namespace NutriTEC.Domain.Entities;

public class Client
{
    // Client stores profile fields that are specific to users registered as clients.
    public int ClientId { get; set; }

    public decimal MaxDailyCalories { get; set; }

    public string Country { get; set; } = string.Empty;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Measure> Measures { get; set; } = new List<Measure>();

    public ICollection<PlanAssignment> PlanAssignments { get; set; } = new List<PlanAssignment>();

    public ICollection<NutritionistClient> NutritionistAssociations { get; set; } = new List<NutritionistClient>();
}
