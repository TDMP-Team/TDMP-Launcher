namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal class LauncherVersionUtility
    {
        internal static string GetLauncherVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion ?? "x.x.x.x";
        }
    }
}
