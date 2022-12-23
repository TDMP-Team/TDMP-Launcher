using System.IO;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class PathUtility
    {
        public static string GetTeardownDirectory(string teardownExePath)
        {
            return Path.GetDirectoryName(teardownExePath);
        }
    }
}
