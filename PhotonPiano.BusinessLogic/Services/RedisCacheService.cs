using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.Interfaces;
using StackExchange.Redis;

namespace PhotonPiano.BusinessLogic.Services;

public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.None
    };

    private readonly IDatabase _database = redis.GetDatabase();

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
        // Add a random duration between 10 and 30 minutes to the expiry
        var random = new Random();
        var additionalMinutes = random.Next(10, 31);
        var adjustedExpiry = expiry.Add(TimeSpan.FromMinutes(additionalMinutes));

        var serializedValue = JsonConvert.SerializeObject(value, JsonSerializerSettings);
        await _database.StringSetAsync(key, serializedValue, adjustedExpiry);
    }
}