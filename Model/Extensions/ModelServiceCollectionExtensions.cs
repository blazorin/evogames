using Microsoft.Extensions.DependencyInjection;

namespace Model.Extensions
{
    public static class ModelServiceCollectionExtensions
    {
        /// <summary>
        /// Registro de servicios del Modelo. ¡Cuidado!
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEvoGamesModelServices(this IServiceCollection services)
        {
            //services.AddDbContext<BlazorStoreContext>();
            //services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}