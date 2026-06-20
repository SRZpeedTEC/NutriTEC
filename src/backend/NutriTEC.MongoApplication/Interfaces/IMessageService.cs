using NutriTEC.MongoApplication.DTOs.Messages;

namespace NutriTEC.MongoApplication.Interfaces;

public interface IMessageService
{
    Task<MessageResponse> SendAsync(SendMessageRequest request, CancellationToken cancellationToken);

    Task<ConversationResponse> GetConversationAsync(int nutritionistCode, int clientId, CancellationToken cancellationToken);

    Task MarkAsReadAsync(int nutritionistCode, int clientId, int readerId, CancellationToken cancellationToken);
}
