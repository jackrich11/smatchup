namespace backend.Security.Authentication;

public interface ITokenService {
    public string GetToken();
    string RefreshToken();
    //method to get claims from expired token
}