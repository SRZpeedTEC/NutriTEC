namespace NutriTEC.Domain.Entities;

public class Recipe
{
    // Recipes are reusable client-owned templates whose calorie total is maintained by SQL Server.
    public int RecipeId { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public decimal TotalCalories { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public ICollection<RecipeProduct> Products { get; set; } = new List<RecipeProduct>();
}
