using System.IO;

namespace TT.Domain.Utilities
{
    public class XmlResourceLoader
    {
        public static string Load(string resourcePath)
        {
            var assembly = typeof (XmlResourceLoader).Assembly;

            var resource = assembly.GetManifestResourceStream(resourcePath);

            if (resource == null)
                throw new ResourceNotFoundException(assembly, resourcePath);

            return new StreamReader(resource).ReadToEnd();
        }
    }
}