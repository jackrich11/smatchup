namespace backend.Models.Requests;

public class CreateMatchupRequest {
    public List<Character> LookingFor { get; set;} = [];
    public List<Character> CanPlay { get; set;} = [];

}