using System.IO;

namespace TeardownMultiplayerLauncher.Core
{
    internal class TeardownPathUtility
    {
        public string TeardownExePath { get; set; } = string.Empty;

        public string GetTeardownDirectory()
        {
            return Path.GetDirectoryName(TeardownExePath);
        }

        public string GetTeardownMultiplayerDllPath()
        {
            return Path.Join(GetTeardownDirectory(), "TDMP.dll");
        }
    }
}
