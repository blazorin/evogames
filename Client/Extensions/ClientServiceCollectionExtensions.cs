using Client.Extensions.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Extensions
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddEvoGamesClientServices(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            return services;
        }
    }
}