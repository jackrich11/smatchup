namespace backend.Models.Responses;

public class TokenResponse {
    public string token_type { get; set; } = null!;
    public string access_token { get; set; } = null!;

    public int expires_in { get; set; } = 0;
    public string refresh_token { get; set; } = null!;

    public string scope { get; set; } = null!;


}

