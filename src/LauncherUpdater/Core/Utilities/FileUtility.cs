using System.IO;
using System.Threading.Tasks;

namespace LauncherUpdater.Core.Utilities
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

        /// <summary>
        /// Moves all files and subfolders from within a source directory into the destination directory, and removes the empty source directory afterwards.
        /// </summary>
        public static void MoveDirectoryContents(string sourceDirectory, string destinationDirectory)
        {
            var directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                var directoryToCreate = directory.Replace(sourceDirectory, destinationDirectory);
                Directory.CreateDirectory(directoryToCreate);
            }

            var filePaths = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                File.Copy(filePath, filePath.Replace(sourceDirectory, destinationDirectory));
                File.Delete(filePath);
            }

            Directory.Delete(sourceDirectory, true);
        }
    }
}
