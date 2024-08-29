namespace backend.Models;

using backend.Types;
using MongoDB.Bson.Serialization.Attributes;

public class Settings {

    [BsonId]
    public required string Username { get; set; }
    public required string Region { get; set; } = Regions.NO_REGION;
    public required string PlayerSkillLevel { get; set; } = SkillLevels.NO_LEVEL_SET;
    public required bool HasDelayMod { get; set; } = false;
}