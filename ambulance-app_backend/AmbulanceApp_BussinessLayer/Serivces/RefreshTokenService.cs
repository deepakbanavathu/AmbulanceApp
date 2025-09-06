using AmbulanceApp.Models.Authentication;
using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using AmbulanceApp_DBContext.DBContext;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Serivces
{
    public class RefreshTokenService : IRefreshToken
    {
        private readonly IRedisService _redis;
        private readonly AmbulanceAppDBContext _db; 

        public RefreshTokenService(IRedisService redis, AmbulanceAppDBContext db)
        {
            _redis = redis;
            _db = db;
        }       

        public RefreshToken GenerateRefreshToken(string userId, int daysValid = 30)
        {
            var plainToken = generateSecureToken();
            var hashed = HashToken(plainToken);

            return new RefreshToken
            {
                Token = plainToken,
                TokenHash = hashed,
                ExpiryDate = DateTime.UtcNow.AddDays(daysValid),
                IsRevoked = false,
                UserId = userId
            };
        }

        private string generateSecureToken(int size = 64)
        {
            var randomNumber = new byte[size];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string HashToken(string token)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
       
        public async Task saveRefreshTokenAsync(RefreshToken _refreshToken)
        {
            var key = $"refresh_token:{_refreshToken.UserId}:{_refreshToken.TokenHash}";
            var ttl = _refreshToken.ExpiryDate - DateTime.UtcNow;

            var tokendata = JsonSerializer.Serialize(new
            {
                _refreshToken.UserId,
                _refreshToken.ExpiryDate
            });

            await _redis.SetAsync(key, tokendata, ttl);            
        }

        public async Task RevokeRefreshToken(string userId, string refreshToken)
        {
            var hashed = HashToken(refreshToken);
            await _redis.RemoveAsync($"refresh_token:{userId}:{hashed}");
        }

        public async Task<bool> ValidateRefreshToken(string userId, string refreshToken)
        {
            var hased = HashToken(refreshToken);
            var storeValue = await _redis.GetAsync($"refresh_token:{userId}:{hased}");

            if (string.IsNullOrEmpty(storeValue)) { return false; }

            var tokenData = JsonSerializer.Deserialize<Dictionary<string, string>>(storeValue);

            return tokenData?["UserId"] == userId;
        }
    }
}
