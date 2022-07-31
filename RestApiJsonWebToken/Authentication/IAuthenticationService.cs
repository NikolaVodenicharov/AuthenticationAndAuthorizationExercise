using RestApiJsonWebToken.Authentication.ResponseModels;
using System.Security.Claims;

namespace RestApiJsonWebToken.Authentication
{
    public interface IAuthenticationService
    {
        public string CreateAccessToken(IEnumerable<Claim> claims);
        public RefreshToken CreateRefreshToken();

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);

        //public ClaimsPrincipal GetPrincipalForExpiredToken(string token);
    }
}
