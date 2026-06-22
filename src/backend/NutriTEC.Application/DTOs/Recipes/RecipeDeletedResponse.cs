namespace NutriTEC.Application.DTOs.Recipes;

public class RecipeDeletedResponse
{
    // Hard-deletion confirmation returns only the removed identifier and safe message.
    public int RecipeId { get; set; }

    public string Message { get; set; } = string.Empty;
}
