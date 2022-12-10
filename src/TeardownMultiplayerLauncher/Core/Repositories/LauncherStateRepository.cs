using Newtonsoft.Json;
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
            var launcherState = JsonConvert.DeserializeObject<LauncherState>(await File.ReadAllTextAsync(LauncherStateFilePath));
            return launcherState.LauncherStateVersion == LauncherState.CurrentLauncherStateVersion ? launcherState : new LauncherState();
        }

        public async Task SaveLauncherStateAsync(LauncherState state)
        {
            await EnsureLauncherStateFileExistsAsync();
            state.LauncherStateVersion = LauncherState.CurrentLauncherStateVersion;
            await File.WriteAllTextAsync(LauncherStateFilePath, JsonConvert.SerializeObject(state));
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
