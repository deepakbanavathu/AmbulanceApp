using Ambulance.Api.Helper;
using Ambulance.Api.MiddleWear;
using AmbulanceApp.Models.Authentication;
using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using AmbulanceApp_BussinessLayer.Serivces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Runtime.CompilerServices;

namespace Ambulance.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration config)
        {
            service.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var connectionString = config.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            });

            service.AddScoped<IAuthenticationService, AuthenticationService>();
            service.AddScoped<IUserRepository, UserRepository>();
            service.AddScoped<IJwtToken, JwtTokenCreation>();
            service.AddScoped<IRefreshToken, RefreshTokenService>();
            service.AddScoped<IRedisService, RedisService>();

            service.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                policy => policy.WithOrigins("https://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod());
            });

            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AmbulanceAPi", Version = "v1" });
            });

            return service;
        }

        public static WebApplication Configuration(this WebApplication app, WebApplicationBuilder builder)
        {
            var config = builder.Configuration;

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("" + config["ApplicationUrl"] + "/swagger/v1/swagger.json", "AmbulanceApp v1");
                });

                app.UseSwagger(option =>
                {
                    option.SerializeAsV2 = true;
                });                        
            }

            app.UseAuthorization();


            #region MiddleWare
            app.UseMiddleware<JwtMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            #endregion

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            return app;
        }
    }
}
