using MongoDB.Driver;
using NutriTEC.MongoDomain.Documents;
using NutriTEC.MongoInfrastructure.Settings;

namespace NutriTEC.MongoInfrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);

        EnsureIndexes();
    }

    public IMongoCollection<Message> Messages => _database.GetCollection<Message>("messages");

    private void EnsureIndexes()
    {
        var indexKeys = Builders<Message>.IndexKeys
            .Ascending(m => m.NutritionistCode)
            .Ascending(m => m.ClientId)
            .Ascending(m => m.SentAt);

        var indexModel = new CreateIndexModel<Message>(
            indexKeys,
            new CreateIndexOptions { Name = "idx_messages_conversation" });

        Messages.Indexes.CreateOne(indexModel);
    }
}
