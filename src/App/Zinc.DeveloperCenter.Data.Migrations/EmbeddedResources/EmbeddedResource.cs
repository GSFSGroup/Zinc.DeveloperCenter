using System.IO;
using System.Reflection;

namespace Zinc.DeveloperCenter.Data.Migrations.EmbeddedResources
{
    /// <summary>
    /// Reads embedded resources.
    /// </summary>
    internal static class EmbeddedResource
    {
        /// <summary>
        /// Reads an embedded seed.
        /// </summary>
        /// <param name="fileName">Filename of the seed.</param>
        /// <returns>Contents of the file.</returns>
        public static string Read(string fileName)
        {
            var resource = $"{typeof(EmbeddedResource).Namespace}.{fileName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            using var textStreamReader = new StreamReader(stream);

            return textStreamReader.ReadToEnd();
        }
    }
}
