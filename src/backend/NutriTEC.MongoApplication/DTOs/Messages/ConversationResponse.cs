namespace NutriTEC.MongoApplication.DTOs.Messages;

public class ConversationResponse
{
    public int NutritionistCode { get; set; }

    public int ClientId { get; set; }

    public List<MessageResponse> Messages { get; set; } = new();
}
