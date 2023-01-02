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
        private static readonly int MaxProcessSearchAttempts = 5;
        private static readonly TimeSpan ProcessSearchInterval = TimeSpan.FromSeconds(1);
        private readonly LauncherState _state;

        public GameLaunchingService(LauncherState state)
        {
            _state = state;
        }

        public Task LaunchTeardownMultiplayerAsync()
        {
            return Task.Run(() =>
            {
                LaunchAndInject();
                WaitForGameToClose();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private void WaitForGameToClose()
        {
            for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
            {
                Thread.Sleep(ProcessSearchInterval); // Search interval.

                var teardownProcessName = Path.GetFileNameWithoutExtension(_state.TeardownExePath);
                var teardownProcess = Process.GetProcessesByName(teardownProcessName).FirstOrDefault();
                if (teardownProcess == null)
                {
                    if (processSearchAttempt < MaxProcessSearchAttempts)
                    {
                        continue;
                    }
                    throw new Exception("Could not find running Teardown process");
                }

                teardownProcess.WaitForExit();
                return;
            }
        }

        private void LaunchAndInject()
        {
            if (!TeardownMultiplayerInjectionUtility.LaunchAndInject(PathUtility.GetTeardownDirectory(_state.TeardownExePath)))
            {
                throw new Exception("Failed to inject TDMP");
            }
        }
    }
}
