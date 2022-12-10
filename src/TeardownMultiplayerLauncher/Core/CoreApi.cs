using TeardownMultiplayerLauncher.Core.Models;
using TeardownMultiplayerLauncher.Core.Repositories;
using TeardownMultiplayerLauncher.Core.Services;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core
{
    internal class CoreApi
    {
        private readonly LauncherConfigRepository _launcherConfigRepository;
        private readonly GameLaunchingService _gameLaunchingService;
        private LauncherConfig _config;

        public CoreApi()
        {
            _launcherConfigRepository = new LauncherConfigRepository();
            _gameLaunchingService = new GameLaunchingService();
            _config = new LauncherConfig();
        }

        public bool LaunchTeardownMultiplayer()
        {
            return _gameLaunchingService.LaunchTeardownMultiplayer(_config.TeardownExePath);
        }

        public string GetTeardownExePath()
        {
            return _config.TeardownExePath;
        }

        public void SetTeardownExePath(string path)
        {
            _config.TeardownExePath = path.Trim();
        }

        public bool? HasSupportedTeardownVersion()
        {
            return GameVersionUtility.HasSupportedTeardownVersion(_config.TeardownExePath);
        }

        public string GetLauncherVersion()
        {
            return LauncherVersionUtility.GetLauncherVersion();
        }

        public void LoadConfig()
        {
            _config = _launcherConfigRepository.GetLauncherConfig();
        }

        public void SaveConfig()
        {
            _launcherConfigRepository.SaveLauncherConfig(_config);
        }
    }
}
