using System.IO;

namespace LauncherUpdater.Core.Utilities
{
    internal static class PathUtility
    {
        public static string GetLauncherDirectory(string launcherExePath)
        {
            return Path.GetDirectoryName(launcherExePath);
        }
    }
}
