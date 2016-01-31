using System.IO;
using System.Xml.Serialization;

namespace TT.Domain.Utilities
{
    public static class XmlExtensions
    {
        public static TOutput ToObject<TOutput>(this string source)
        {
            var xmlSerializer = new XmlSerializer(typeof(TOutput));
            return (TOutput)xmlSerializer.Deserialize(new StringReader(source));
        }
    }
}