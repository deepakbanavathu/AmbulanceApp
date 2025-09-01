using AmbulanceApp.Models;

namespace Ambulance.Api.Interface
{
    public interface IRefreshToken
    {
        RefreshToken GenerateRefreshToken();       
        string HashToken(string token);
        bool ValidateRefreshToken(string userId, string refreshToken);
        void RevokeRefreshToken(string userId, string refreshToken);
        void saveRefreshToken(RefreshToken _refreshToken);

    }
}
