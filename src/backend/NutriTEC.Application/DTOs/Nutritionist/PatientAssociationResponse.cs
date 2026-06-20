namespace NutriTEC.Application.DTOs.Nutritionist;

public class PatientAssociationResponse
{
    public int NutritionistCode { get; set; }

    public int ClientId { get; set; }

    public DateOnly AssociationDate { get; set; }

    public string Message { get; set; } = string.Empty;
}
