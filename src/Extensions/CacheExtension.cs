using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TelegramTeamprojectBot.Extensions;

public static class CacheExtension
{
    public static async Task SetRecordAsync<T>(this IDistributedCache cache,
        string recordId,
        T data,
        TimeSpan? absoluteExpireTime = null,
        TimeSpan? slidingExpireTime = null)
    {
        var options = new DistributedCacheEntryOptions();

        options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromDays(180);
        options.SlidingExpiration = slidingExpireTime;

        var jsonData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recordId, jsonData, options);
    }

    public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache,
        string recordId)
    {
        var jsonData = await cache.GetStringAsync(recordId);

        if (jsonData is null)
        {
            return default;
        }

        if (typeof(T) == typeof(string)) return (T)(object)jsonData;
        return JsonSerializer.Deserialize<T>(jsonData);
    }

    public static void RemoveRecord(this IDistributedCache cache, string recordId) =>
        cache.Remove(recordId);
}