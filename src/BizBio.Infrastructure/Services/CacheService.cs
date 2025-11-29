using System.Collections.Concurrent;
using BizBio.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BizBio.Infrastructure.Services;

/// <summary>
/// Implementation of ICacheService using IMemoryCache
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, byte> _keys;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _keys = new ConcurrentDictionary<string, byte>();
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        if (_cache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        var value = await factory();
        if (value != null)
        {
            Set(key, value, expiration);
        }

        return value;
    }

    public T? Get<T>(string key) where T : class
    {
        return _cache.TryGetValue(key, out T? value) ? value : null;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };

        options.RegisterPostEvictionCallback((evictedKey, evictedValue, reason, state) =>
        {
            _keys.TryRemove(evictedKey.ToString()!, out _);
        });

        _cache.Set(key, value, options);
        _keys.TryAdd(key, 0);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _keys.TryRemove(key, out _);
    }

    public void RemoveByPattern(string pattern)
    {
        var keysToRemove = _keys.Keys
            .Where(k => k.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var key in keysToRemove)
        {
            Remove(key);
        }
    }

    public void Clear()
    {
        foreach (var key in _keys.Keys.ToList())
        {
            Remove(key);
        }
    }
}