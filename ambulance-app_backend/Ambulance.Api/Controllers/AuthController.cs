using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ambulance.Api.Interface;

namespace Ambulance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IRefreshToken refresh;

        public AuthController(IConfiguration config, IRefreshToken _refresh)
        {
            _config = config;
            refresh = _refresh;
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // Here you would typically validate the refresh token and issue a new JWT
            // For demonstration, we'll just return a new token if the refresh token is "valid_refresh_token"
            var refreshToken = GetRefreshTokenFromDb(request.RefreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return Unauthorized(new { error = "invalid_refresh_token" });
            }

            var newAccessToken = refresh.GenerateAccessToken(refreshToken.UserId, _config);

            var newRefreshToken = refresh.GenerateRefreshToken();
            SaveRefreshTokenToDb(refreshToken.UserId, newRefreshToken);

            return Ok(new
            {
                access_token = newAccessToken,
                refresh_token = newRefreshToken
            });

        }

    }
}
