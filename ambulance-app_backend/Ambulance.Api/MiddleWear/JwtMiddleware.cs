using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Security.Claims;

namespace Ambulance.Api.MiddleWear
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
                AttachUserToContext(context, token);
            await _next(context);
        }

        private (bool IsValid, string? Error) AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var jwtSettings = _config.GetSection("Jwt");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(jwtSettings["key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                }, out SecurityToken validatedToken);


                var jwtToken = (JwtSecurityToken)validatedToken;

                //Attach user information to context if needed
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                var role = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role)?.Value;

                context.Items["User"] = new { UserId = userId, Role = role };
                return (true, null);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException ("token_expired");
            }
            catch (SecurityTokenValidationException)
            {
                throw new UnauthorizedAccessException("Invalid_token");
            }
            catch
            {
                throw new UnauthorizedAccessException("unauthorised");
            }
        }
    }
}
