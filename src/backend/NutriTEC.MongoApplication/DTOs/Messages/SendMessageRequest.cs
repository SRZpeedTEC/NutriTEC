namespace NutriTEC.MongoApplication.DTOs.Messages;

public class SendMessageRequest
{
    public int NutritionistCode { get; set; }

    public int ClientId { get; set; }

    public int SenderId { get; set; }

    public string SenderType { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
