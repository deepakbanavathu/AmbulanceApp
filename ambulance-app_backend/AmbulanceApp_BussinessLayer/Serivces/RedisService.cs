using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Serivces
{
    public  class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private IDatabase redisDb => _redis.GetDatabase();

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await redisDb.StringSetAsync(key,value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            var value = await redisDb.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task RemoveAsync(string key)
        {
            await redisDb.KeyDeleteAsync(key);
        }
    }
}
