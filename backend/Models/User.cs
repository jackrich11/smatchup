using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class User {
    [BsonId]
    public required ulong Id { get; set; }
    public required string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsBanned { get; set; }
    public DateTime Created { get; set; }

}