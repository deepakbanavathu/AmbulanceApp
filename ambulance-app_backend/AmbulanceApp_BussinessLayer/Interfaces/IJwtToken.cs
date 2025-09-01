using System.Security.Claims;

namespace Ambulance.Api.Interface
{
    public interface IJwtToken
    {
        string GenerateAccessToken(string userId, string role);
        ClaimsPrincipal? ValidateAccessToken(string token);
    }
}
