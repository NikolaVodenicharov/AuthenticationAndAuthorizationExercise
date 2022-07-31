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

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.Username = register.Username;

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

            List<Claim> tokenClaims = new()
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, AdminRole)
            };

            AuthenticatedResponse response = new(
                authenticationService.CreateAccessToken(tokenClaims));

            return Ok(response);
        }
    }
}
