namespace Api.Caching.Abstractions
{
    public interface ICacheableQuery
    {
        string Key { get; }

        ExpirationOptions Options { get; }
    }
}
