using MongoDB.Driver;
using NutriTEC.MongoDomain.Documents;
using NutriTEC.MongoApplication.Interfaces;
using NutriTEC.MongoInfrastructure.Persistence;

namespace NutriTEC.MongoInfrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MessageRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task SendAsync(Message message, CancellationToken cancellationToken)
    {
        await _context.Messages.InsertOneAsync(message, cancellationToken: cancellationToken);
    }

    public async Task<List<Message>> GetConversationAsync(
        int nutritionistCode,
        int clientId,
        CancellationToken cancellationToken)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.NutritionistCode, nutritionistCode),
            Builders<Message>.Filter.Eq(m => m.ClientId, clientId));

        var sort = Builders<Message>.Sort.Ascending(m => m.SentAt);

        return await _context.Messages
            .Find(filter)
            .Sort(sort)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(
        int nutritionistCode,
        int clientId,
        int readerId,
        CancellationToken cancellationToken)
    {
        // Mark as read only the messages NOT sent by the reader (messages sent to them).
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.NutritionistCode, nutritionistCode),
            Builders<Message>.Filter.Eq(m => m.ClientId, clientId),
            Builders<Message>.Filter.Ne(m => m.SenderId, readerId),
            Builders<Message>.Filter.Eq(m => m.IsRead, false));

        var update = Builders<Message>.Update.Set(m => m.IsRead, true);

        await _context.Messages.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
    }
}
