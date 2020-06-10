using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using NLog;

namespace Honeybee.Core
{
    public static class ImageUtils
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Loads image resource from Embedded Resources. Used for creating Ribbon Icons.
        /// </summary>
        /// <param name="a">Assembly that the resource is embedded in.</param>
        /// <param name="name">Name of the resource.</param>
        /// <returns>BitmapImage object.</returns>
        public static BitmapImage LoadImage(Assembly a, string name)
        {
            var img = new BitmapImage();
            try
            {
                var resourceName = a.GetManifestResourceNames().FirstOrDefault(x => x.Contains(name));
                var stream = a.GetManifestResourceStream(resourceName);

                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
            return img;
        }
    }
}
