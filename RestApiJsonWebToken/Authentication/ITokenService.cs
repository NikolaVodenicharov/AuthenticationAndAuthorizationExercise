using System.Security.Claims;

namespace RestApiJsonWebToken.Authentication
{
    public interface ITokenService
    {
        public string CreateAccessToken(IEnumerable<Claim> claims);
        public string CreateRefreshToken();
        public ClaimsPrincipal GetPrincipalForExpiredToken(string token);
    }
}
