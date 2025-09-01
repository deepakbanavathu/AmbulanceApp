using System.Runtime.CompilerServices;
using Ambulance.Api.Models;

namespace Ambulance.Api.Interface
{
    public interface IRefreshToken
    {
        RefreshToken GenerateRefreshToken();
        string GenerateAccessToken(string userId, IConfiguration config);

    }
}
