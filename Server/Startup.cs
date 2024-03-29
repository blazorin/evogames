using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model.Data;
using Model.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public class Startup
    {
        public const string JwtSecretKey = "71c51d71df6a1b9c93bab5da5e89d183258defaa401551635195247847c16c28";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();

            services.AddEvoGamesModelServices();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(JwtSecretKey)
                        ),
                        RequireExpirationTime = true,
                        ValidIssuer = "EvoGamesServer",
                        ValidAudience = "EvoGamesClient"
                    };
                });

            // CORS
            services.AddCors();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var db = new EvoGamesContext())
            {
                db.Database.EnsureCreated();
            }

            //CORS
            app.UseCors(builder =>
            {
                builder.WithOrigins("https://localhost:5001");
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            // Handles exceptions and generates a custom response body
            app.UseExceptionHandler("/errors/500");

            // Handles non-success status codes with empty body
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                /* // Disabling WebAssembly hosting in backend. Now served by CDN.*/
                // app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            /* // Disabling WebAssembly hosting in backend. Now served by CDN.*/
            // app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                /* // Disabling WebAssembly hosting in backend. Now served by CDN.*/
                // endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}