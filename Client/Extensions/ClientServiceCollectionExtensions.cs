using Microsoft.Extensions.DependencyInjection;

namespace Client.Extensions
{
    public static class ClientServiceCollectionExtensions
    {
        public static IServiceCollection AddEvoGamesClientServices(this IServiceCollection services)
        {
            return services;
        }
    }
}