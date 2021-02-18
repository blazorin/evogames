using System;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using Client.Extensions;
using Client.Extensions.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            builder.Services.AddEvoGamesClientServices();

            // Authentication and Authorization
            builder.Services.AddAuthorizationCore();
            builder.Services.AddOptions();

            // I18N
            CultureInfo.DefaultThreadCurrentCulture =
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            await builder.Build().RunAsync();
        }
    }
}