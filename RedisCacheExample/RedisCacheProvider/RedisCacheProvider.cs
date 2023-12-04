using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisCacheExample.Interface;
using RedisCacheExample.Model;
using System.Text;

namespace RedisCacheExample.RedisCacheProvider;

public class RedisCacheProvider : IRedisCacheProvider
{
    private IDistributedCache _distributedCache;

    public RedisCacheProvider(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async void Insert<T>(string cacheKey, T result, TimeSpan expiresIn)
    {
        try
        {
            var distributedCacheEntryOptions = new DistributedCacheEntryOptions();
            distributedCacheEntryOptions.AbsoluteExpirationRelativeToNow = expiresIn;
            distributedCacheEntryOptions.SlidingExpiration = null;
            var jsonData = JsonConvert.SerializeObject(result);
            String s = jsonData;
            await _distributedCache.SetStringAsync(cacheKey, s, distributedCacheEntryOptions);
        }
        catch (Exception ex)
        {
        }
    }

    public async Task<T> TryGet<T>(string cacheKey)
    {
        T result = default(T);
        try
        {
            var jsonData = await _distributedCache.GetStringAsync(cacheKey);
                result = JsonConvert.DeserializeObject<T>(jsonData);
                return result;
        }
        catch (Exception ex)
        {
            return result;
        }
    }

    public async Task Remove(string cacheKey)
    {
         await _distributedCache.RemoveAsync(cacheKey);
    }
}

