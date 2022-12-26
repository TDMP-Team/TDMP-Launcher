using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;

namespace LauncherUpdater.Core.Repositories
{
    internal class LauncherStateRepository
    {
        private static readonly string LauncherStateFilePath = "state.json";

        public async Task<UpdaterState> GetLauncherStateAsync()
        {
            await EnsureLauncherStateFileExistsAsync();
            try
            {
                var launcherState = JsonConvert.DeserializeObject<UpdaterState>(await File.ReadAllTextAsync(LauncherStateFilePath));
                return launcherState.LauncherStateVersion == UpdaterState.CurrentLauncherStateVersion ? launcherState : new UpdaterState();
            }
            catch (Exception ex) when (
                ex is JsonReaderException ||
                ex is NullReferenceException
            )
            {
                return new UpdaterState();
            }
        }

        public async Task SaveLauncherStateAsync(UpdaterState state)
        {
            await EnsureLauncherStateFileExistsAsync();
            state.LauncherStateVersion = UpdaterState.CurrentLauncherStateVersion;
            try
            {
                await File.WriteAllTextAsync(LauncherStateFilePath, JsonConvert.SerializeObject(state));
            }
            catch (IOException)
            {
                return; // TODO: When we add logging, we should log when this exception gets swallowed.
            }
        }

        private async Task EnsureLauncherStateFileExistsAsync()
        {
            if (!File.Exists(LauncherStateFilePath))
            {
                await File.WriteAllTextAsync(LauncherStateFilePath, JsonConvert.SerializeObject(new UpdaterState()));
            }
        }
    }
}
