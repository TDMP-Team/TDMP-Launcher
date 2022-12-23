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
        private static readonly int MaxProcessSearchAttempts = 10;
        private static readonly string TeardownProcessName = "teardown";

        public async Task LaunchTeardownMultiplayerAsync(string teardownExePath)
        {
            await Task.Run(() =>
            {
                LaunchTeardown();
                WaitForGameAndInject(teardownExePath);
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private void LaunchTeardown()
        {
            Process.Start(
                new ProcessStartInfo("steam://rungameid/1167630")
                {
                    UseShellExecute = true,
                    Verb = "open",
                }
            );
        }

        private void WaitForGameAndInject(string teardownExePath)
        {
            for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
            {
                Thread.Sleep(3000); // Search interval delay.

                var teardownProcess = Process.GetProcessesByName(TeardownProcessName).FirstOrDefault();
                if (teardownProcess == null)
                {
                    if (processSearchAttempt < MaxProcessSearchAttempts)
                    {
                        continue;
                    }
                    throw new Exception("Could not find running Teardown process");
                }

                if (!InjectTeardownMultiplayer(teardownProcess, teardownExePath))
                {
                    throw new Exception("Failed to inject TDMP");
                }

                teardownProcess = Process.GetProcessesByName(TeardownProcessName).FirstOrDefault(); // Search for teardown process again after injection because the old Process object gets corrupted.
                if (teardownProcess != null)
                {
                    teardownProcess.WaitForExit();
                }
                return;
            }
        }

        private bool InjectTeardownMultiplayer(Process teardownProcess, string teardownExePath)
        {
            try
            {
                return DllInjectionUtility.InjectDLL(PathUtility.GetTeardownMultiplayerDllPath(teardownExePath), teardownProcess);
            }
            catch
            {
                teardownProcess.Kill();
                return false;
            }
        }
    }
}
