using TeardownMultiplayerLauncher.Core.Models;
using TeardownMultiplayerLauncher.Core.Repositories;
using TeardownMultiplayerLauncher.Core.Services;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core
{
    internal class CoreApi
    {
        private readonly StateRepository _stateRepository;
        private readonly GameLaunchingService _gameLauncherService;
        private State _state;

        public CoreApi()
        {
            _stateRepository = new StateRepository();
            _gameLauncherService = new GameLaunchingService();
            _state = new State();
        }

        public bool LaunchTeardownMultiplayer()
        {
            return _gameLauncherService.LaunchTeardownMultiplayer(_state.TeardownExePath);
        }

        public string GetTeardownExePath()
        {
            return _state.TeardownExePath;
        }

        public void SetTeardownExePath(string path)
        {
            _state.TeardownExePath = path.Trim();
        }

        public bool? HasSupportedTeardownVersion()
        {
            return GameVersionUtility.HasSupportedTeardownVersion(_state.TeardownExePath);
        }

        public string GetLauncherVersion()
        {
            return LauncherVersionUtility.GetLauncherVersion();
        }

        public void LoadState()
        {
            _state = _stateRepository.GetState();
        }

        public void SaveState()
        {
            _stateRepository.SaveState(_state);
        }
    }
}
