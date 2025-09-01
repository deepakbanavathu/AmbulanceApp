using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ambulance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // Here you would typically validate the refresh token and issue a new JWT
            // For demonstration, we'll just return a new token if the refresh token is "valid_refresh_token"
            var refreshToken = GetRefreshTokenFromDb(request.RefreshToken);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate < DateTime.UtcNow)
            {
                var jwtSettings = _config.GetSection("Jwt");
                var newToken = new
                {
                    Token = "new_jwt_token",
                    ExpiresIn = 3600 // 1 hour
                };
                return Ok(newToken);
            }
            else
            {
                return Unauthorized(new { Message = "Invalid refresh token" });
            }
        }
    }
}
