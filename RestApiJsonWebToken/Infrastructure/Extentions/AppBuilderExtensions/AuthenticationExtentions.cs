using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RestApiJsonWebToken.Infrastructure.Extentions.AppBuilderExtensions
{
    public static class AuthenticationExtentions
    {
        public static void CustomizeAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            AddAuthenticationAndJwtBearer(services, configuration);
        }

        private static void AddAuthenticationAndJwtBearer(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = CreateTokenValidationParameter(configuration);
                    });
        }
        private static TokenValidationParameters CreateTokenValidationParameter(IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = CreateSymmetricSecurityKey(configuration),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            return tokenValidationParameters;
        }
        private static SymmetricSecurityKey CreateSymmetricSecurityKey(IConfiguration configuration)
        {
            var secretKey = configuration
               .GetSection("JwtSettings:Secret")
               .Value;

            var secretKeyBytes = Encoding
                .UTF8
                .GetBytes(secretKey);

            var symmetricSecurityKey = new SymmetricSecurityKey(secretKeyBytes);

            return symmetricSecurityKey;
        }
    }
}
