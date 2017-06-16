using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class DistributeCacheExtensions
    {
        public static Task SetObjectAsync(this IDistributedCache cache, string key, object obj)
        {
            return cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static void SetObject(this IDistributedCache cache, string key, object obj)
        {
            cache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key) where T : class
        {
            var bytes = await cache.GetAsync(key).ConfigureAwait(false);

            if (bytes == null)
            {
                throw new ArgumentException("Key not found in cache");
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static T GetObject<T>(this IDistributedCache cache, string key) where T : class
        {
            var bytes = cache.Get(key);

            if (bytes == null)
            {
                throw new ArgumentException("Key not found in cache");
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }
    }
}