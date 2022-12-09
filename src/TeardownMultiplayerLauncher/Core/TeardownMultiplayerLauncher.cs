using System.Linq;
using System.Threading;

namespace TeardownMultiplayerLauncher.Core
{
    internal class Launcher
    {
        private readonly TeardownPathUtility _pathUtility;
        private readonly DllInjectionUtility _dllInjectionUtility;

        public Launcher(TeardownPathUtility pathUtility, DllInjectionUtility dllInjectionUtility)
        {
            _pathUtility = pathUtility;
            _dllInjectionUtility = dllInjectionUtility;
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
