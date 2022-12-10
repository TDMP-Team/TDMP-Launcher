using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class GameLaunchingService
    {
        public async Task LaunchTeardownMultiplayerAsync(string teardownExePath)
        {
            await Task.Run(() =>
            {
                LaunchTeardown(teardownExePath);
                Thread.Sleep(5000); // TODO: detect when game is actually ready to inject into.
                var teardownProcess = Process.GetProcessesByName("teardown").FirstOrDefault();
                if (teardownProcess == null)
                {
                    throw new Exception("Could not find running Teardown process");
                }
                InjectTeardownMultiplayer(teardownProcess, teardownExePath);
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
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
