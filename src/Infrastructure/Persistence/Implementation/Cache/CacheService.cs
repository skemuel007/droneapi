using Application.Contracts.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Persistence.Implementation.Cache;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _redisCache;

    public CacheService(IDistributedCache redisCache)
    {
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
    }
    
    public async Task<T?> GetData<T>(string key)
    {
        var data = await _redisCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(data))
            return JsonConvert.DeserializeObject<T>(data);
        return default;
    }

    public async Task SetData<T>(string key, T value)
    {
        await _redisCache.SetStringAsync(key, JsonConvert.SerializeObject(value));
    }

    public async Task RemoveData(string key)
    {
        await _redisCache.RemoveAsync(key);
    }
}