using RestApiJsonWebToken.Authentication;

namespace RestApiJsonWebToken.Infrastructure.Extensions.ServiceCollection
{
    // Extensions for IServiceCollection of WebApplicationBuilder.
    // All customizations about dependency injection are extracted here.
    public static class DependenciesExtensions
    {
        public static void CustomizeDependencies(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }
    }
}
