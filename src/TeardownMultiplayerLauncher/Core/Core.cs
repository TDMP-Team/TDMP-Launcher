namespace TeardownMultiplayerLauncher.Core
{
    internal class Core
    {
        private readonly TeardownPathUtility _pathUtility;
        private readonly GameVersionUtility _gameVersionUtility;
        private readonly DllInjectionUtility _dllInjectionUtility;
        private readonly Launcher _launcher;

        public Core()
        {
            _pathUtility = new TeardownPathUtility();
            _gameVersionUtility = new GameVersionUtility(_pathUtility);
            _dllInjectionUtility = new DllInjectionUtility();
            _launcher = new Launcher(_pathUtility, _dllInjectionUtility);
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

        public bool LaunchTeardownMultiplayer()
        {
            return _launcher.LaunchTeardownMultiplayer();
        }
    }
}
