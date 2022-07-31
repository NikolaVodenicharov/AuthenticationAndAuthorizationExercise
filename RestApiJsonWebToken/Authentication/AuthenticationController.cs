using Microsoft.AspNetCore.Mvc;
using RestApiJsonWebToken.Authentication.RequestModels;
using RestApiJsonWebToken.Authentication.ResponseModels;
using RestApiJsonWebToken.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestApiJsonWebToken.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public const string AdminRole = "Admin";
        private const string RefreshTokenCookie = "RefreshTokenCookie";

        public static User user = new();

        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost(nameof(Register))]
        public ActionResult<User> Register(RegisterRequest register)
        {
            using var hmac = new HMACSHA512();

            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));

            List<Claim> tokenClaims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, AdminRole)
            };

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.Username = register.Username;
            user.Claims = tokenClaims;

            return Ok(user);
        }

        // Login which return token for that user
        [HttpPost(nameof(Login))]
        public ActionResult<AuthenticatedResponse> Login(LoginRequest login)
        {
            if (user.Username != login.Username)
            {
                return BadRequest("User not found.");
            }

            var isPasswordValid = this.authenticationService.VerifyPasswordHash(
                    login.Password,
                    user.PasswordHash,
                    user.PasswordSalt);

            if (!isPasswordValid)
            {
                return BadRequest("Wrong password.");
            }

/*            List<Claim> tokenClaims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, AdminRole)
            };*/

            AuthenticatedResponse response = new(
                authenticationService.CreateAccessToken(user.Claims));

            SetRefreshTokenToCookie();
            return Ok(response);
        }

        // refresh AccessToken and RefreshToken
        [HttpPost(nameof(RefreshToken))]
        public ActionResult<string> RefreshToken()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookie];

            if (!user.RefreshTokenString.Equals(refreshToken))
            {
                return Unauthorized("Invalid refresh token.");
            }

            if(user.RefreshTokenExpirationDate < DateTime.UtcNow)
            {
                return Unauthorized("Refresh token is expired.");
            }

            var accessToken = this.authenticationService.CreateAccessToken(user.Claims);
            AuthenticatedResponse response = new(accessToken);

            SetRefreshTokenToCookie();

            return Unauthorized(response);
        }

        private void SetRefreshTokenToCookie()
        {
            var refreshToken = this.authenticationService.CreateRefreshToken();

            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = refreshToken.ExpirationDate
            };

            Response.Cookies.Append(RefreshTokenCookie, refreshToken.TokenString, cookieOptions);

            user.RefreshTokenString = refreshToken.TokenString;
            user.RefreshTokenExpirationDate = refreshToken.ExpirationDate;
        }
    }
}
