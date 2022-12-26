using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;
using LauncherUpdater.Core.Repositories;
using LauncherUpdater.Core.Services;
using LauncherUpdater.Core.Utilities;

namespace LauncherUpdater.Core
{
    internal class CoreApi
    {
        private LauncherStateRepository? _launcherStateRepository;
        private LauncherLaunchingService? _launcherLaunchingService;
        private LauncherUpdateService? _launcherUpdateService;
        private UpdaterState? _state;

        public async Task InitializeAsync()
        {
            _launcherStateRepository = new LauncherStateRepository();
            _state = await _launcherStateRepository.GetLauncherStateAsync();
            _launcherLaunchingService = new LauncherLaunchingService(_state);
            _launcherUpdateService = new LauncherUpdateService(_state);
            await SetLauncherExePathAsync(Directory.GetCurrentDirectory() + "/bin/TeardownMultiplayerLauncher.exe");
            DoOtherStuffAsync();
        }

        public async Task DoOtherStuffAsync()
        {
            await SetUpLatestLauncherReleaseAsync();
            await _launcherLaunchingService.LaunchLauncherAsync();
        }

        public string GetLauncherExePath()
        {
            return _state.LauncherExePath;
        }

        public string GetSelectedCultureCode()
        {
            return _state.SelectedCultureCode;
        }

        public Task SetLauncherExePathAsync(string path)
        {
            _state.LauncherExePath = path.Trim();
            return _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public string GetLauncherVersion()
        {
            return UpdaterVersionUtility.GetUpdaterVersion();
        }

        public async Task SetUpLatestLauncherReleaseAsync()
        {
            await _launcherUpdateService.SetUpLatestReleaseAsync(PathUtility.GetLauncherDirectory(_state.LauncherExePath));
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }

        public float GetPercentageDone()
        {
            return _state.PercentageDone;
        }

        public string GetCurrentTask()
        {
            return _state.CurrentTask;
        }

        public async Task SetCurrentTaskAsync(string task)
        {
            _state.CurrentTask = task;
            await _launcherStateRepository.SaveLauncherStateAsync(_state);
        }
    }
}
