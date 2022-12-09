using System.Diagnostics;
using System.Linq;
using System.Threading;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class GameLaunchingService
    {
        public bool LaunchTeardownMultiplayer(string teardownExePath)
        {
            LaunchTeardown(teardownExePath);
            Thread.Sleep(5000);
            var teardownProcess = Process.GetProcessesByName("teardown").FirstOrDefault();
            if (teardownProcess == null)
            {
                return false;
            }
            return InjectTeardownMultiplayer(teardownProcess, teardownExePath);
        }

        private Process LaunchTeardown(string teardownExePath)
        {
            return Process.Start(teardownExePath);
        }

        private bool InjectTeardownMultiplayer(Process teardownProcess, string teardownExePath)
        {
            try
            {
                return DllInjectionUtility.InjectDLL(TeardownPathUtility.GetTeardownMultiplayerDllPath(teardownExePath), teardownProcess);
            }
            catch
            {
                teardownProcess.Kill();
                return false;
            }
        }
    }
}
