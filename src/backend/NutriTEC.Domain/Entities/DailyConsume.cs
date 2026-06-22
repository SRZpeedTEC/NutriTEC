namespace NutriTEC.Domain.Entities;

public class DailyConsume
{
    // Daily consumption stores the trigger-maintained calorie summary for one client and date.
    public int ClientId { get; set; }

    public DateOnly ConsumeDate { get; set; }

    public decimal TotalCalories { get; set; }

    public Client Client { get; set; } = null!;

    public ICollection<DailyMealTime> MealTimes { get; set; } = new List<DailyMealTime>();
}
