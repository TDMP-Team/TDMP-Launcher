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
            return Task.Run(LaunchAndInjectAndWaitForGameToClose).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private void LaunchAndInjectAndWaitForGameToClose()
        {
            if (!TeardownMultiplayerInjectionUtility.LaunchAndInjectAndWaitForGameToClose(PathUtility.GetTeardownDirectory(_state.TeardownExePath)))
            {
                throw new Exception("Failed to inject TDMP");
            }
        }
    }
}
