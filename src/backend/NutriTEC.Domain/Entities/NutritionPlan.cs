namespace NutriTEC.Domain.Entities;

public class NutritionPlan
{
    // Nutrition plans contain the persisted plan summary fields used by client login responses.
    public int PlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int NutritionistCode { get; set; }

    public Nutritionist? Nutritionist { get; set; }

    public ICollection<PlanMealTime> PlanMealTimes { get; set; } = new List<PlanMealTime>();

    public ICollection<PlanAssignment> PlanAssignments { get; set; } = new List<PlanAssignment>();
}
