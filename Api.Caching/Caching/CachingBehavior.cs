using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Caching.Abstractions;
using Api.Caching.Extensions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;


namespace Api.Caching.Caching
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IDistributedCache cache;
        private readonly ILogger logger;

        public CachingBehavior(IDistributedCache cache, ILogger logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is not ICacheableQuery cacheable)
                return await next();

            if (cacheable.Key == null)
                throw new ArgumentNullException(nameof(cacheable.Key));

            var (exist, returnValue) = await cache.TryGetAsync<TResponse>(cacheable.Key, cancellationToken);

            if (exist)
            {
                logger.Information($"Key '{{{cacheable.Key}}}' found. Returning from cache object {{{typeof(TRequest).Name}}}");
                return returnValue;
            }

            var value = await next();

            await cache.SetAsync(cacheable.Key, value, cacheable.Options.GetCacheEntryOptions(), cancellationToken);

            logger.Information($"Insert to cache object {{{typeof(TRequest).Name}}} with key {{{cacheable.Key}}}");
            return value;
        }
    }
}
