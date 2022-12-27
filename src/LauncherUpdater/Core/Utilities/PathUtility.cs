using System.IO;

namespace LauncherUpdater.Core.Utilities
{
    internal static class PathUtility
    {
        public static readonly string TeardownLauncherDirectory = Path.Combine(Directory.GetCurrentDirectory(), "bin/");

        public static readonly string TeardownLauncherExePath = Path.Combine(TeardownLauncherDirectory, "TeardownMultiplayerLauncher.exe");
    }
}