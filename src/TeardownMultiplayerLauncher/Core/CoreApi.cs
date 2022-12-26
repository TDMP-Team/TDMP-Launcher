using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Repositories;
using TeardownMultiplayerLauncher.Core.Services;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core
{
    internal class CoreApi
    {
        private LauncherStateRepository? _launcherStateRepository;
        private GameLaunchingService? _gameLaunchingService;
        private TeardownMultiplayerUpdateService? _teardownMultiplayerUpdateService;
        private LauncherState? _state;

        public async Task InitializeAsync()
        {
            _launcherStateRepository = new LauncherStateRepository();
            _state = await _launcherStateRepository.GetLauncherStateAsync();
            _gameLaunchingService = new GameLaunchingService(_state);
            _teardownMultiplayerUpdateService = new TeardownMultiplayerUpdateService(_state);
            await DetectAndSetTeardownExePathAsync();
        }

        public async Task LaunchTeardownMultiplayer()
        {
            await _gameLaunchingService.LaunchTeardownMultiplayerAsync();
        }

        public string GetTeardownExePath()
        {
            return _state.TeardownExePath;
        }

        public async Task SetTeardownExePathAsync(string path)
        {
            _state.TeardownExePath = path.Trim();
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public string GetSupportedTeardownVersion()
        {
            return GameVersionUtility.SupportedTeardownVersion;
        }

        public bool? HasSupportedTeardownVersion()
        {
            return GameVersionUtility.HasSupportedTeardownVersion(_state.TeardownExePath);
        }

        public string GetLauncherVersion()
        {
            return LauncherVersionUtility.GetLauncherVersion();
        }

        public string GetInstalledTeardownMultiplayerVersion()
        {
            var teardownMultiplayerFileVersion = FileVersionUtility.GetFileVersion(PathUtility.GetTeardownMultiplayerDllPath(_state.TeardownExePath));
            return string.IsNullOrWhiteSpace(teardownMultiplayerFileVersion) ? _state.TeardownMultiplayerUpdateState.InstalledVersion : teardownMultiplayerFileVersion;
        }

        public async Task SetUpLatestTeardownMultiplayerReleaseAsync()
        {
            await _teardownMultiplayerUpdateService.SetUpLatestReleaseAsync(PathUtility.GetTeardownDirectory(_state.TeardownExePath));
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public TimeSpan GetInjectionDelay()
        {
            return _state.InjectionDelay;
        }

        public async Task SetInjectionDelayAsync(TimeSpan injectionDelay)
        {
            _state.InjectionDelay = injectionDelay;
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public void OpenDiscordServer()
        {
            Process.Start(
                new ProcessStartInfo(_state.DiscordUrl)
                {
                    UseShellExecute = true,
                    Verb = "open",
                }
            );
        }

        private async Task DetectAndSetTeardownExePathAsync()
        {
            if (string.IsNullOrWhiteSpace(GetTeardownExePath()))
            {
                var maybeTeardownExePath = TeardownPathDetectionUtility.MaybeGetTeardownExePath();
                if (!string.IsNullOrWhiteSpace(maybeTeardownExePath))
                {
                    await SetTeardownExePathAsync(maybeTeardownExePath);
                }
            }
        }
    }
}
