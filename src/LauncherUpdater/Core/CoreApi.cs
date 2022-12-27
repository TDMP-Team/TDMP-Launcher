using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;
using LauncherUpdater.Core.Repositories;
using LauncherUpdater.Core.Services;
using LauncherUpdater.Core.Utilities;

namespace LauncherUpdater.Core
{
    internal class CoreApi
    {
        private UpdaterStateRepository? _launcherStateRepository;
        private LauncherLaunchingService? _launcherLaunchingService;
        private LauncherUpdateService? _launcherUpdateService;
        private LauncherUpdaterState? _state;

        public async Task InitializeAsync()
        {
            _launcherStateRepository = new UpdaterStateRepository();
            _state = await _launcherStateRepository.GetUpdaterStateAsync();
            _launcherLaunchingService = new LauncherLaunchingService(_state);
            _launcherUpdateService = new LauncherUpdateService(_state);
        }

        public string GetLauncherVersion()
        {
            return UpdaterVersionUtility.GetUpdaterVersion();
        }

        public float GetPercentageDone()
        {
            return _state.Progress;
        }

        public string GetCurrentTask()
        {
            return _state.CurrentTask;
        }

        public async Task SetUpAndLaunchLauncherAsync()
        {
            await SetUpLatestLauncherReleaseAsync();
            await _launcherLaunchingService.LaunchLauncherAsync();
        }

        private async Task SetUpLatestLauncherReleaseAsync()
        {
            await _launcherUpdateService.SetUpLatestReleaseAsync();
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

    }
}
