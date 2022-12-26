﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LauncherUpdater.Core.Models.State;
using LauncherUpdater.Core.Utilities;

namespace LauncherUpdater.Core.Services
{
    internal class LauncherLaunchingService
    {
        private static readonly int MaxProcessSearchAttempts = 30;
        private static readonly TimeSpan ProcessSearchInterval = TimeSpan.FromSeconds(1);
        private static readonly string LauncherProcessName = "TeardownMultiplayerLauncher";
        private readonly UpdaterState _state;

        public LauncherLaunchingService(UpdaterState state)
        {
            _state = state;
        }

        public Task LaunchLauncherAsync()
        {
            _state.CurrentTask = "Launching...";
            return Task.Run(() =>
            {
                LaunchTeardown();
                WaitForLauncherAndClose();
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
                new ProcessStartInfo(_state.LauncherExePath)
                {
                    UseShellExecute = true,
                    Verb = "open",
                    WorkingDirectory = PathUtility.GetLauncherDirectory(_state.LauncherExePath)
                }
            );
        }

        private void WaitForLauncherAndClose()
        {
            for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
            {
                Thread.Sleep(ProcessSearchInterval); // Search interval.

                var launcherProcess = Process.GetProcessesByName(LauncherProcessName).FirstOrDefault();
                if (launcherProcess == null)
                {
                    if (processSearchAttempt < MaxProcessSearchAttempts)
                    {
                        continue;
                    }
                    throw new Exception("Could not find running Launcher process");
                }

                launcherProcess = Process.GetProcessesByName(LauncherProcessName).FirstOrDefault(); // Search for teardown process again after injection because the old Process object gets corrupted.
                if (launcherProcess != null)
                {
                    Process.GetCurrentProcess().Kill();
                }
                return;
            }
        }
    }
}
