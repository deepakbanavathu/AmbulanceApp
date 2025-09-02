using Ambulance.Api.Helper;
using AmbulanceApp.Models.Authentication;
using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using AmbulanceApp_BussinessLayer.Serivces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
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

            return service;
        }
    }
}
