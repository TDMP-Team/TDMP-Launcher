using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models;
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
        private LocalizationService? _localizationService;
        private LauncherState? _state;

        public async Task InitializeAsync()
        {
            _launcherStateRepository = new LauncherStateRepository();
            _state = await _launcherStateRepository.GetLauncherStateAsync();
            _gameLaunchingService = new GameLaunchingService(_state);
            _teardownMultiplayerUpdateService = new TeardownMultiplayerUpdateService(_state);
            _localizationService = new LocalizationService(_state);
            await DetectAndSetTeardownExePathAsync();
        }

        public Task LaunchTeardownMultiplayer()
        {
            return _gameLaunchingService.LaunchTeardownMultiplayerAsync();
        }

        public string GetTeardownExePath()
        {
            return _state.TeardownExePath;
        }

        public string GetSelectedCultureCode()
        {
            return _state.SelectedCultureCode;
        }

        public Task SetSelectedCultureCodeAsync(string cultureCode)
        {
            _state.SelectedCultureCode = cultureCode;
            return _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public Task<LocaleData> GetLocaleDataAsync(string cultureCode)
        {
            return _localizationService.GetLocaleDataAsync(cultureCode);
        }

        public IEnumerable<string> GetSupportedCultureCodes()
        {
            return _localizationService.GetSupportedCultureCodes();
        }

        public Task SetTeardownExePathAsync(string path)
        {
            _state.TeardownExePath = path.Trim();
            return _launcherStateRepository.SaveLauncherStateAsync(_state);
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

        public Task SetInjectionDelayAsync(TimeSpan injectionDelay)
        {
            _state.InjectionDelay = injectionDelay;
            return _launcherStateRepository.SaveLauncherStateAsync(_state);
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

        public void OpenLauncherReleasePage()
        {
            Process.Start(
                new ProcessStartInfo($"https://github.com/TDMP-Team/TDMP-Launcher-Public/releases/tag/{GetLauncherVersion()}")
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
