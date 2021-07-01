using Microsoft.Extensions.DependencyInjection;
using Model.Data;
using Model.Mapping;
using Model.Services;

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
            // Essential
            services.AddDbContext<EvoGamesContext>();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            //User
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IProfileServices, ProfileServices>();


            return services;
        }

        /*
        public static IServiceCollection AddEvoGamesRateLimitServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.Configure<ClientRateLimitOptions>(options =>
            {
                options.GeneralRules = new List<RateLimitRule>
                {
                    new()
                    {
                        Endpoint = "*",
                        Period = "10m",
                        Limit = 7500,
                    }
                };
                options.HttpStatusCode = 429;
                
            });

            return services;
        }
        */
    }
}