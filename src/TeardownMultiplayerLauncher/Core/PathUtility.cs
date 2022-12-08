using System.IO;
using System.Reflection;

namespace TeardownMultiplayerLauncher.Core
{
    internal class PathUtility
    {
        public string TeardownDirectory { get; set; } = string.Empty;

        public string GetTeardownExePath()
        {
            return Path.Join(TeardownDirectory, "teardown.exe");
        }

        public string GetTeardownMultiplayerDllPath()
        {
            return Path.Join(TeardownDirectory, "TDMP.dll");
        }
    }
}
