using System.IO;
using System.Reflection;

namespace Honeybee.Core
{
    public static class Resources
    {
        /// <summary>
        /// Return string stream of an embedded resource.
        /// </summary>
        /// <param name="assembly">Assembly to get the resource from.</param>
        /// <param name="fileName">Name of the resource.</param>
        /// <returns>String stream of the resource.</returns>
        public static string StreamEmbeddedResource(Assembly assembly, string fileName)
        {
            using (var stream = assembly.GetManifestResourceStream(fileName))
            {
                if (stream == null) return string.Empty;

                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
