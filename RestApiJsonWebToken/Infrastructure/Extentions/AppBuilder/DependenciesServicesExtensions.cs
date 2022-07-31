using RestApiJsonWebToken.Authentication;

namespace RestApiJsonWebToken.Infrastructure.Extentions.AppBuilder
{
    public static class DependenciesServicesExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }
    }
}
