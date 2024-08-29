using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

[BsonIgnoreExtraElements]
public class Matchup {
    public required string MatchupId { get; set; }
    public required string Creator { get; set; } //user who opens the matchup
    public string Visitor { get; set; } = null!; //cant be same as the creator
    public required DateTime Created = DateTime.Now;
    public List<Character> CanPlay { get; set; } = [];
    public List<Character> LookingFor { get; set; } = [];

    override public string ToString() {
        return $"Id is {MatchupId}";
    }

    //Other things can be looking for matchups, characters that can be played, skill level
}