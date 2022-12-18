using System;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Repositories;
using TeardownMultiplayerLauncher.Core.Services;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core
{
    internal class CoreApi
    {
        private LauncherStateRepository? _launcherConfigRepository;
        private GameLaunchingService? _gameLaunchingService;
        private TeardownMultiplayerUpdateService? _teardownMultiplayerUpdateService;
        private LauncherState? _state;

        public async Task InitializeAsync()
        {
            _launcherConfigRepository = new LauncherStateRepository();
            _gameLaunchingService = new GameLaunchingService();
            _state = await _launcherConfigRepository.GetLauncherStateAsync();
            _teardownMultiplayerUpdateService = new TeardownMultiplayerUpdateService(_state.TeardownMultiplayerUpdateState);
        }

        public async Task LaunchTeardownMultiplayer()
        {
            await _gameLaunchingService.LaunchTeardownMultiplayerAsync(_state.TeardownExePath);
        }

        public string GetTeardownExePath()
        {
            return _state.TeardownExePath;
        }

        public void SetTeardownExePath(string path)
        {
            _state.TeardownExePath = path.Trim();
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
            return _state.TeardownMultiplayerUpdateState.InstalledVersion;
        }

        public async Task SetUpLatestTeardownMultiplayerReleaseAsync()
        {
            await _teardownMultiplayerUpdateService.SetUpLatestReleaseAsync(TeardownPathUtility.GetTeardownDirectory(_state.TeardownExePath));
            await _launcherConfigRepository.SaveLauncherStateAsync(_state);
        }
    }
}
