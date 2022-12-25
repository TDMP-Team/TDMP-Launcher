using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Repositories;
using TeardownMultiplayerLauncher.Core.Services;
using TeardownMultiplayerLauncher.Core.Utilities;
using Gameloop.Vdf;
using System.IO;
using Gameloop.Vdf.Linq;
using System.Linq;
using Gameloop.Vdf.JsonConverter;

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

            // Only do this check if there isn't an already selected path
            if (_state.TeardownExePath == null || _state.TeardownExePath.Length == 0)
            {
                if (!await TryGetTeardownPathAsync("SOFTWARE\\Valve\\Steam"))
                {
                    await TryGetTeardownPathAsync("SOFTWARE\\Wow6432Node\\Valve\\Steam");
                }
            }
        }

        public async Task<bool> TryGetTeardownPathAsync(string regKey)
        {
            RegistryKey? key = Registry.LocalMachine.OpenSubKey(regKey);
            if (key != null)
            {
                object? o = key.GetValue("InstallPath");
                if (o != null)
                {
                    VProperty volvo = VdfConvert.Deserialize(File.ReadAllText(o.ToString() + "/config/libraryfolders.vdf"));
                    foreach(var location in volvo.Value.ToList())
                    {
                        foreach (var item in location.ToJson().Children())
                        {
                            string installPath = item.SelectToken("path").ToString();
                            if (Directory.Exists(installPath + "\\steamapps\\common\\Teardown"))
                            {
                                await SetTeardownExePathAsync(installPath + "\\steamapps\\common\\Teardown\\teardown.exe");
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
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
    }
}
