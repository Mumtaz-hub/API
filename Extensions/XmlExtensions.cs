using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Extensions
{
    public static class XmlExtensions
    {
        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        };

        private static readonly XmlSerializerNamespaces Namespaces = new XmlSerializerNamespaces();


        public static string ToXml<T>(this T data, bool omitXmlDeclaration = false)
        {
            if (data == null)
                return string.Empty;

            return omitXmlDeclaration ? DeSerializeWithOutNamespaces(data) : DeSerialize(data);
        }

        private static string DeSerialize(object data)
        {
            using (var textWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(data.GetType());
                xmlSerializer.Serialize(textWriter, data);
                return textWriter.ToString();
            }
        }

        private static string DeSerializeWithOutNamespaces(object data)
        {
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, WriterSettings))
            {
                var xmlSerializer = new XmlSerializer(data.GetType());
                Namespaces.Add(string.Empty, string.Empty);
                xmlSerializer.Serialize(writer, data, Namespaces);
                return sw.ToString();
            }
        }

        public static T ToObject<T>(this string xmlString) where T : class, new()
        {
            if (string.IsNullOrEmpty(xmlString))
                return new T();

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xmlString))
            {
                try { return (T)serializer.Deserialize(reader); }
                catch { return null; } // Could not be deserialized to this type.
            }
        }
    }
}
