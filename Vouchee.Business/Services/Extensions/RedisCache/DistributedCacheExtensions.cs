using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vouchee.Business.Services.Extensions.RedisCache
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<dynamic>(this IDistributedCache cache, string recordId, dynamic data)
        {

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if (jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

        public static async Task DeleteKeyAsync(this IDistributedCache cache, string recordId)
        {
            await cache.RemoveAsync(recordId);
        }
    }
}
