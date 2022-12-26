using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class LauncherStateRepository
    {
        private static readonly string LauncherStateFilePath = "state.json";

        public async Task<LauncherState> GetLauncherStateAsync()
        {
            await EnsureLauncherStateFileExistsAsync();
            try
            {
                var launcherState = JsonConvert.DeserializeObject<LauncherState>(await File.ReadAllTextAsync(LauncherStateFilePath));
                return launcherState.LauncherStateVersion == LauncherState.CurrentLauncherStateVersion ? launcherState : new LauncherState();
            } catch(Exception ex) when (ex is JsonReaderException||
                                        ex is NullReferenceException)
            {
                return new LauncherState();
            }
        }

        public async Task SaveLauncherStateAsync(LauncherState state)
        {
            await EnsureLauncherStateFileExistsAsync();
            state.LauncherStateVersion = LauncherState.CurrentLauncherStateVersion;
            try
            {
                await File.WriteAllTextAsync(LauncherStateFilePath, JsonConvert.SerializeObject(state));
            } catch(IOException)
            {
                return;
            }
        }

        private async Task EnsureLauncherStateFileExistsAsync()
        {
            if (!File.Exists(LauncherStateFilePath))
            {
                await File.WriteAllTextAsync(LauncherStateFilePath, JsonConvert.SerializeObject(new LauncherState()));
            }
        }
    }
}
