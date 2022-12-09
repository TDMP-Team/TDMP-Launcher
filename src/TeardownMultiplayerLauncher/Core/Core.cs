using System;

namespace TeardownMultiplayerLauncher.Core
{
    internal class Core
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly TeardownPathUtility _pathUtility;
        private readonly GameVersionUtility _gameVersionUtility;
        private readonly DllInjectionUtility _dllInjectionUtility;
        private readonly Launcher _launcher;

        public Core()
        {
            _settingsRepository = new SettingsRepository();
            _pathUtility = new TeardownPathUtility();
            _gameVersionUtility = new GameVersionUtility(_pathUtility);
            _dllInjectionUtility = new DllInjectionUtility();
            _launcher = new Launcher(_pathUtility, _dllInjectionUtility);
        }

        public bool LaunchTeardownMultiplayer()
        {
            return _launcher.LaunchTeardownMultiplayer();
        }

        public string GetTeardownExePath()
        {
            return _pathUtility.TeardownExePath;
        }

        public void SetTeardownExePath(string path)
        {
            var trimmedPath = path.Trim();
            _pathUtility.TeardownExePath = trimmedPath;
        }

        public bool? HasSupportedTeardownVersion()
        {
            return _gameVersionUtility.HasSupportedTeardownVersion();
        }

        public string GetLauncherVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }

        public void LoadSettings()
        {
            var settings = _settingsRepository.GetSettings();
            _pathUtility.TeardownExePath = settings.TeardownExePath;
        }

        public void SaveSettings()
        {
            _settingsRepository.SaveSettings(
                new Settings
                {
                    TeardownExePath = _pathUtility.TeardownExePath,
                }
            );
        }
    }
}
