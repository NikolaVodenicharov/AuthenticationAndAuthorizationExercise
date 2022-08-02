using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestApiJsonWebToken.Configuration;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace RestApiJsonWebToken.Infrastructure.Extensions.ServiceCollection
{
    // Extensions for IServiceCollection of WebApplicationBuilder.
    // All customizations about swagger are extracted here.
    public static class SwaggerExtensions
    {
        public static void CustomizeSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.ConfigureSwagger();
        }

        //Swagger options are adding field for locking the token.
        //There we must write "bearer + the token" just like in the description below.
        private static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "oauth2",
                    new OpenApiSecurityScheme
                    {
                        Description = "Standart Authorization header using Bearer scheme (\"bearer {token}\")",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }
    }
}
