using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Api.Models;

public class RefreshToken
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } 
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; } = null;
    public string? ReplacedByToken { get; set; } = null;
    public bool IsActive => RevokedAt == null && DateTime.UtcNow <= ExpiresAt;
}