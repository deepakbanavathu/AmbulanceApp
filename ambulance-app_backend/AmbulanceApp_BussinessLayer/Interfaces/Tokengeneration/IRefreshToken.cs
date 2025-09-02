using AmbulanceApp.Models.Authentication;

namespace AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration
{
    public interface IRefreshToken
    {
        RefreshToken GenerateRefreshToken(string userId, int daysValid = 30);       
        string HashToken(string token);
        Task<bool> ValidateRefreshToken(string userId, string refreshToken);
        Task RevokeRefreshToken(string userId, string refreshToken);
        Task saveRefreshTokenAsync(RefreshToken _refreshToken);

    }
}
