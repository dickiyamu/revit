using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeybee.Core
{
    public static class FileUtils
    {
        public static bool TryDeleteFile(string file)
        {
            if (!File.Exists(file)) return true;
            try
            {
                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
