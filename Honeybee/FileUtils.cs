using System.IO;

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
        
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }

            return false;
        }
    }
}
