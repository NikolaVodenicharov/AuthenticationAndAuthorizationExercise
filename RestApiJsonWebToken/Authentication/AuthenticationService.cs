using Microsoft.IdentityModel.Tokens;
using RestApiJsonWebToken.Authentication.ResponseModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestApiJsonWebToken.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private const int AccessTokenExpirationMinutes = 15;
        private const int RefreshTokenExpirationDays = 7;

        private readonly IConfiguration configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
                signingCredentials: CreateCredentials());

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenString = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            return tokenString;
        }

        public RefreshToken CreateRefreshToken()
        {
            var randomNumber = new Byte[64];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);

            var tokenString = Convert.ToBase64String(randomNumber);
            var expires = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

            RefreshToken refreshToken = new(
                tokenString,
                expires);


            return refreshToken;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var isMatching = computedHash.SequenceEqual(passwordHash);

            return isMatching;
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
