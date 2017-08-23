using System;
using System.Collections.Generic;
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

            if (bytes == null) return null;

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static T GetObject<T>(this IDistributedCache cache, string key) where T : class
        {
            var bytes = cache.Get(key);

            if (bytes == null) return null;

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public static async Task<object> GetObjectAsync(this IDistributedCache cache, string key)
        {
            var bytes = await cache.GetAsync(key).ConfigureAwait(false);

            if (bytes == null) return null;

            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
        }

        public static object GetObject(this IDistributedCache cache, string key)
        {
            var bytes = cache.Get(key);

            if (bytes == null) return null;

            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
        }

        public static async Task AddObjectToHashSetAsync<T>(this IDistributedCache cache, string key, T obj) where T : class
        {
            var collection = await cache.GetObjectAsync<HashSet<T>>(key).ConfigureAwait(false);

            if (collection == null)
            {
                await cache.SetObjectAsync(key, new HashSet<T> { obj }).ConfigureAwait(false);
            }
            else
            {
                collection.Add(obj);
                await cache.SetObjectAsync(key, collection).ConfigureAwait(false);
            }
        }

        public static void AddObjectToHashSet<T>(this IDistributedCache cache, string key, T obj) where T : class
        {
            var collection = cache.GetObject<HashSet<T>>(key);

            if (collection == null)
            {
                cache.SetObject(key, new HashSet<T> { obj });
            }
            else
            {
                collection.Add(obj);
                cache.SetObject(key, collection);
            }
        }

        public static async Task RemoveFromHashSetAsync<T>(this IDistributedCache cache, string key, T obj) where T : class
        {
            var collection = await cache.GetObjectAsync<HashSet<T>>(key).ConfigureAwait(false);

            if (collection == null) return;

            collection.Remove(obj);
        }

        public static void RemoveFromHashSet<T>(this IDistributedCache cache, string key, T obj) where T : class
        {
            var collection = cache.GetObject<HashSet<T>>(key);

            if (collection == null) return;

            collection.Remove(obj);
        }
    }
}