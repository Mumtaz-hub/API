using System;

namespace Api.Caching.Abstractions
{
    public class ExpirationOptions
    {
        public ExpirationOptions()
        {

        }

        public ExpirationOptions(TimeSpan? absoluteExpirationRelativeToNow)
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
        }

        public DateTimeOffset? AbsoluteExpiration { get; set; }


        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }


        public TimeSpan? SlidingExpiration { get; set; }
    }
}
