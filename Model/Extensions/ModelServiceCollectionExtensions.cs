using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Data;
using Model.Mapping;
using Model.Services;
using Shared;

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
    }
}