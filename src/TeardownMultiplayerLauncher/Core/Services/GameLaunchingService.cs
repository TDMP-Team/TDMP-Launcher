using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class GameLaunchingService
    {
        private static readonly int MaxProcessSearchAttempts = 10;
        private static readonly TimeSpan ProcessSearchInterval = TimeSpan.FromSeconds(5);
        private static readonly string TeardownProcessName = "teardown";
        private readonly LauncherState _state;

        public GameLaunchingService(LauncherState state)
        {
            _state = state;
        }

        public async Task LaunchTeardownMultiplayerAsync()
        {
            await Task.Run(() =>
            {
                LaunchTeardown();
                WaitForGameAndInject();
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

        private void WaitForGameAndInject()
        {
            for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
            {
                Thread.Sleep(ProcessSearchInterval); // Search interval.

                var teardownProcess = Process.GetProcessesByName(TeardownProcessName).FirstOrDefault();
                if (teardownProcess == null)
                {
                    if (processSearchAttempt < MaxProcessSearchAttempts)
                    {
                        continue;
                    }
                    throw new Exception("Could not find running Teardown process");
                }

                Thread.Sleep(_state.InjectionDelay); // TODO: Reliably check memory if Teardown Game object is initialized. Slower PCs might take longer for Teardown to fully initialize and be ready to inject into.
                if (!InjectTeardownMultiplayer(teardownProcess))
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

        private bool InjectTeardownMultiplayer(Process teardownProcess)
        {
            try
            {
                return DllInjectionUtility.InjectDLL(PathUtility.GetTeardownMultiplayerDllPath(_state.TeardownExePath), teardownProcess);
            }
            catch
            {
                teardownProcess.Kill();
                return false;
            }
        }
    }
}
