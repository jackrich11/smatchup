namespace backend.Models.Requests;

public class SetSettingsRequest {
    public required string Username { get; set; }
    public required string Region { get; set; }
    public required string SkillLevel { get; set; }
    public required bool HasDelayMod { get; set; }
}