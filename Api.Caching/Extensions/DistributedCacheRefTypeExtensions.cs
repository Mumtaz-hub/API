using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Caching.Serialization;
using Api.Caching.Serialization.Providers;
using Microsoft.Extensions.Caching.Distributed;

namespace Api.Caching.Extensions
{
    public static class DistributedCacheRefTypeExtensions
    {
        private static ISerializer Serializer
        {
            get
            {
                SerializerProvider.SetDefaultSerializer(new MessagePackSerializer());
                //SerializerProvider.SetDefaultSerializer(new NewtonsoftJsonSerializer());
                return SerializerProvider.Default;
            }
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            cache.Set(key, Serializer.Serialize(value), options);
        }

        public static T Get<T>(this IDistributedCache cache, string key) where T : class
        {
            var val = cache.Get(key);

            return val == null
                ? null
                : Serializer.Deserialize<T>(val);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, CancellationToken token = default) where T : class
        {
            await SetAsync(cache, key, value, new DistributedCacheEntryOptions(), token);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await cache.SetAsync(key, Serializer.Serialize(value), options, token);
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default) where T : class
        {
            var val = await cache.GetAsync(key, token);

            return val == null
                ? null
                : Serializer.Deserialize<T>(val);
        }

        public static bool TryGet<T>(this IDistributedCache cache, string key, out T returnValue)
        {
            var val = cache.Get(key);

            if (val == null)
            {
                returnValue = default;
                return false;
            }

            returnValue = Serializer.Deserialize<T>(val);
            return true;
        }

        public static async Task<(bool exist, T returnValue)> TryGetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            var val = await cache.GetAsync(key, token);
            if (val == null)
                return (false, default);


            var value = Serializer.Deserialize<T>(val);
            return (true, value);
        }

        public static void RemoveKey(this IDistributedCache cache, string key)
        {
            cache.Remove(key);
        }

        public static async Task RemoveKeyAsync(this IDistributedCache cache, string key, CancellationToken token = default)
        {
            await cache.RemoveAsync(key, token);
        }

        public static void RemoveKey(this IDistributedCache cache, List<string> keys)
        {
            foreach (var key in keys)
            {
                cache.Remove(key);
            }
        }

        public static async Task RemoveKeyAsync(this IDistributedCache cache, List<string> keys, CancellationToken token = default)
        {
            foreach (var key in keys)
            {
                await cache.RemoveKeyAsync(key, token);
            }
        }
    }
}