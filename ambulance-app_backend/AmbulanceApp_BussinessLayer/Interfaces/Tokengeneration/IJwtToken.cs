using System.Security.Claims;

namespace AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration
{
    public interface IJwtToken
    {
        string GenerateAccessToken(string userId, string role);      
    }
}
