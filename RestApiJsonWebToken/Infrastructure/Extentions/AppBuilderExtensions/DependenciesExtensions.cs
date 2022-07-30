using RestApiJsonWebToken.Authentication;

namespace RestApiJsonWebToken.Infrastructure.Extentions.AppBuilderExtensions
{
    public static class DependenciesExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
        }
    }
}
