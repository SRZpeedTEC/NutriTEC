namespace NutriTEC.MongoApplication.DTOs.Messages;

public class MessageResponse
{
    public string Id { get; set; } = string.Empty;

    public int SenderId { get; set; }

    public string SenderType { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}
