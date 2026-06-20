using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NutriTEC.MongoDomain.Documents;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int NutritionistCode { get; set; }

    public int ClientId { get; set; }

    public int SenderId { get; set; }

    public string SenderType { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }
}
