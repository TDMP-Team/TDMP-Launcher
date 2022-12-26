namespace LauncherUpdater.Core.Utilities
{
    internal class UpdaterVersionUtility
    {
        internal static string GetUpdaterVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion ?? "x.x.x.x";
        }
    }
}
