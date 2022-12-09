using System.Linq;
using System.Threading;

namespace TeardownMultiplayerLauncher.Core
{
    internal class LauncherCore
    {
        private readonly GameVersionUtility _gameVersionUtility;
        private readonly PathUtility _pathUtility;
        private readonly DllInjectionUtility _dllInjectionUtility;

        public LauncherCore()
        {
            _pathUtility = new PathUtility();
            _gameVersionUtility = new GameVersionUtility(_pathUtility);
            _dllInjectionUtility = new DllInjectionUtility();
        }

        public void SetTeardownExePath(string path)
        {
            var trimmedPath = path.Trim();
            _pathUtility.TeardownExePath = trimmedPath;
        }

        public bool? HasSupportedTeardownVersion()
        {
            return _gameVersionUtility.HasSupportedTeardownVersion();
        }

        public string GetLauncherVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }

        public bool LaunchTeardownMultiplayer()
        {
            LaunchTeardown();
            Thread.Sleep(5000);
            var teardownProcess = System.Diagnostics.Process.GetProcessesByName("teardown").FirstOrDefault();
            if (teardownProcess == null)
            {
                return false;
            }
            return InjectTeardownMultiplayer(teardownProcess);
        }

        private System.Diagnostics.Process LaunchTeardown()
        {
            return System.Diagnostics.Process.Start(_pathUtility.TeardownExePath);
        }

        private bool InjectTeardownMultiplayer(System.Diagnostics.Process teardownProcess)
        {
            try
            {
                return _dllInjectionUtility.InjectDLL(_pathUtility.GetTeardownMultiplayerDllPath(), teardownProcess);
            }
            catch
            {
                teardownProcess.Kill();
                return false;
            }
        }
    }
}
