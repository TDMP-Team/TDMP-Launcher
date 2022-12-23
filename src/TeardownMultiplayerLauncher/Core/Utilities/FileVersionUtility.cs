using System.Diagnostics;
using System.IO;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class FileVersionUtility
    {
        public static string GetFileVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            return FileVersionInfo.GetVersionInfo(filePath)?.FileVersion ?? string.Empty;
        }
    }
}
