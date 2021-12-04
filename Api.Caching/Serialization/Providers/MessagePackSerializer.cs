using MessagePack;
using MessagePack.Resolvers;

namespace Api.Caching.Serialization.Providers
{
    public class MessagePackSerializer : ISerializer
    {
        static MessagePackSerializer()
        {
            var resolver = CompositeResolver.Create(
                NativeDateTimeResolver.Instance,
               ContractlessStandardResolver.Instance,
               StandardResolver.Instance
           );

            MessagePack.MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        }

        public byte[] Serialize<T>(T obj)
        {
            return MessagePack.MessagePackSerializer.Serialize(obj);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(bytes);
        }
    }
}