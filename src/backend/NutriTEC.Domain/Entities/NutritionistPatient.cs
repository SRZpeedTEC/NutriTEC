namespace NutriTEC.Domain.Entities;

public class NutritionistClient
{
    public int NutritionistCode { get; set; }

    public int ClientId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public Nutritionist Nutritionist { get; set; } = null!;

    public Client Client { get; set; } = null!;
}
