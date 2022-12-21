using System.IO;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class PathUtility
    {
        public static string GetTeardownDirectory(string teardownExePath)
        {
            return Path.GetDirectoryName(teardownExePath);
        }

        public static string GetTeardownMultiplayerDllPath(string teardownExePath)
        {
            return Path.Join(GetTeardownDirectory(teardownExePath), "TDMP.dll");
        }
    }
}
