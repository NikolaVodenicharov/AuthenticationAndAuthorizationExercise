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
            var tokenValidationParameters = CreateTokenValidationParameter(configuration);

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
                        options.TokenValidationParameters = tokenValidationParameters;
                    });
        }

        private static TokenValidationParameters CreateTokenValidationParameter(IConfiguration configuration)
        {
            var secretKey = configuration
               .GetSection("JwtSettings:Secret")
               .Value;

            var secretKeyBytes = Encoding
                .UTF8
                .GetBytes(secretKey);

            var symmetricSecurityKey = new SymmetricSecurityKey(secretKeyBytes);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false
            };

            return tokenValidationParameters;
        }
    }
}
