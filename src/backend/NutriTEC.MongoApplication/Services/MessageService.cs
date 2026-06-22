using NutriTEC.MongoDomain.Documents;
using NutriTEC.MongoApplication.DTOs.Messages;
using NutriTEC.MongoApplication.Interfaces;

namespace NutriTEC.MongoApplication.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageResponse> SendAsync(SendMessageRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("El contenido del mensaje no puede estar vacio.");

        if (request.SenderType != "NUTRITIONIST" && request.SenderType != "CLIENT")
            throw new ArgumentException("El tipo de remitente debe ser NUTRITIONIST o CLIENT.");

        var message = new Message
        {
            NutritionistCode = request.NutritionistCode,
            ClientId = request.ClientId,
            SenderId = request.SenderId,
            SenderType = request.SenderType.ToUpperInvariant(),
            Content = request.Content.Trim(),
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _messageRepository.SendAsync(message, cancellationToken);

        return ToResponse(message);
    }

    public async Task<ConversationResponse> GetConversationAsync(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetConversationAsync(nutritionistCode, clientId, cancellationToken);

        return new ConversationResponse
        {
            NutritionistCode = nutritionistCode,
            ClientId = clientId,
            Messages = messages.Select(ToResponse).ToList()
        };
    }

    public Task MarkAsReadAsync(int nutritionistCode, int clientId, int readerId, CancellationToken cancellationToken)
    {
        return _messageRepository.MarkAsReadAsync(nutritionistCode, clientId, readerId, cancellationToken);
    }

    private static MessageResponse ToResponse(Message m) => new()
    {
        Id = m.Id,
        SenderId = m.SenderId,
        SenderType = m.SenderType,
        Content = m.Content,
        SentAt = m.SentAt,
        IsRead = m.IsRead
    };
}
