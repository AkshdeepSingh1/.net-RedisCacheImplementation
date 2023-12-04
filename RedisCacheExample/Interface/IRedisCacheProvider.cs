namespace RedisCacheExample.Interface;

public interface IRedisCacheProvider
{
    void Insert<T>(string cacheKey, T result, TimeSpan expiresIn);
    Task<T> TryGet<T>(string cacheKey);
    Task Remove(string cacheKey);
}

