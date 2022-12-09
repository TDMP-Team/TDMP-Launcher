using System.Linq;
using System.Threading;

namespace TeardownMultiplayerLauncher.Core
{
    internal class LauncherCore
    {
        private readonly GameVersionUtility _gameVersionUtility;
        private readonly PathUtility _pathUtility;

        public LauncherCore()
        {
            _pathUtility = new PathUtility();
            _gameVersionUtility = new GameVersionUtility(_pathUtility);
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
                var processSharp = new Process.NET.ProcessSharp(teardownProcess);
                processSharp.ModuleFactory.Inject(_pathUtility.GetTeardownMultiplayerDllPath(), false);
                return true;
            }
            catch
            {
                teardownProcess.Kill();
                return false;
            }
        }
    }
}
