using NutriTEC.MongoDomain.Documents;

namespace NutriTEC.MongoApplication.Interfaces;

public interface IMessageRepository
{
    Task SendAsync(Message message, CancellationToken cancellationToken);

    Task<List<Message>> GetConversationAsync(int nutritionistCode, int clientId, CancellationToken cancellationToken);

    Task MarkAsReadAsync(int nutritionistCode, int clientId, int readerId, CancellationToken cancellationToken);
}
