using System;
using Api.Caching.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace Api.Caching.Extensions
{
    public static class ExpirationOptionsExtensions
    {
        public static DistributedCacheEntryOptions GetCacheEntryOptions(this ExpirationOptions source)
        {
            return new()
            {
                AbsoluteExpiration = source.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = source.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromSeconds(60),
                SlidingExpiration = source.SlidingExpiration
            };
        }
    }
}
