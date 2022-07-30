using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestApiJsonWebToken.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: CreateCredentials());

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenString = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            return tokenString;
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new Byte[32];

            using(var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalForExpiredToken(string token)
        {
            throw new NotImplementedException();
        }

        private SigningCredentials CreateCredentials()
        {
            var credentias = new SigningCredentials(
                CreateSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha512Signature);

            return credentias;
        }
        private SymmetricSecurityKey CreateSymmetricSecurityKey()
        {
            var secretKey = configuration
                .GetSection("JwtSettings:Secret")
                .Value;

            var secretKeyBytes = Encoding
                .UTF8
                .GetBytes(secretKey);

            var key = new SymmetricSecurityKey(secretKeyBytes);

            return key;
        }
    }
}
