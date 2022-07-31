namespace RestApiJsonWebToken.Authentication.ResponseModels
{
    public record RefreshToken(string TokenString, DateTime ExpirationDate);
}
