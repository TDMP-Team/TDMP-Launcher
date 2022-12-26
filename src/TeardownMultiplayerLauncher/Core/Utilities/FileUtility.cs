using System.IO;
using System.Threading.Tasks;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class FileUtility
    {
        /// <summary>
        /// Creates a file and any directories in its path that are missing.
        /// </summary>
        public static async Task CreateDirectoriesAndFileAsync(string filePath, string fileContents, bool shouldOverwriteFile = false)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(filePath) || shouldOverwriteFile)
            {
                await File.WriteAllTextAsync(filePath, fileContents);
            }
        }
    }
}
