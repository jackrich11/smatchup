using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Session {
    [BsonId]
    public string SessionId { get; set; } = null!;
    public string UserID { get; set; } = null!;
}