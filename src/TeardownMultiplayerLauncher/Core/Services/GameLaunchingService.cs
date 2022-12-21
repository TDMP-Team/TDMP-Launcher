using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class GameLaunchingService
    {
        private static readonly int MaxProcessSearchAttempts = 10;

        public async Task LaunchTeardownMultiplayerAsync(string teardownExePath)
        {
            var processName = Path.GetFileNameWithoutExtension(teardownExePath);
            await Task.Run(async () =>
            {
                Process.Start(teardownExePath);
                for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
                {
                    Thread.Sleep(3000); // Search interval delay.
                    var teardownProcess = Process.GetProcessesByName(processName).FirstOrDefault(); // Search for teardown process again to get around issue where Steam DRM re-launches game.
                    if (teardownProcess == null)
                    {
                        if (processSearchAttempt < MaxProcessSearchAttempts)
                        {
                            continue;
                        }
                        throw new Exception("Could not find running Teardown process");
                    }
                    Thread.Sleep(3000); // TODO: detect when game is actually ready to inject into. This just gives the game some time to "warm up" before injecting.
                    if (!InjectTeardownMultiplayer(teardownProcess, teardownExePath))
                    {
                        throw new Exception("Failed to inject TDMP");
                    }
                    teardownProcess = Process.GetProcessesByName(processName).FirstOrDefault(); // Search for teardown process again after injection because the old Process object gets corrupted.
                    if (teardownProcess != null)
                    {
                        teardownProcess.WaitForExit();
                    }
                    return;
                }
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
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
