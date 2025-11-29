namespace BizBio.Core.Interfaces;

/// <summary>
/// Service for caching data with expiration support
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value or adds it if not present
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Gets a cached value
    /// </summary>
    T? Get<T>(string key) where T : class;

    /// <summary>
    /// Sets a cached value
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Removes a cached value
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Removes all cached values matching a pattern
    /// </summary>
    void RemoveByPattern(string pattern);

    /// <summary>
    /// Clears all cached values
    /// </summary>
    void Clear();
}
