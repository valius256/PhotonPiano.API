
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.Interfaces;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services
{
    public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
    {
        private readonly IDatabase _database = redis.GetDatabase();

        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.None
        };

        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var data = await _database.StringGetAsync(key);

            return data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<T>(data!);
        }

        public async Task SaveAsync<T>(string key, T value, TimeSpan expiry) where T : class
        {
            var serializedValue = JsonConvert.SerializeObject(value, JsonSerializerSettings);
            await _database.StringSetAsync(key, serializedValue, expiry);
        }
    }
}
