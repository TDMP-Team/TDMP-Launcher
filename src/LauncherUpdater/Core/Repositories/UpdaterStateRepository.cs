using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;

namespace LauncherUpdater.Core.Repositories
{
    internal class UpdaterStateRepository
    {
        private static readonly string UpdaterStateFilePath = "state.json";

        public async Task<LauncherUpdaterState> GetUpdaterStateAsync()
        {
            await EnsureUpdaterStateFileExistsAsync();
            try
            {
                var updaterState = JsonConvert.DeserializeObject<LauncherUpdaterState>(await File.ReadAllTextAsync(UpdaterStateFilePath));
                return updaterState.UpdaterStateVersion == LauncherUpdaterState.CurrentUpdaterStateVersion ? updaterState : new LauncherUpdaterState();
            }
            catch (Exception ex) when (
                ex is JsonReaderException ||
                ex is NullReferenceException
            )
            {
                return new LauncherUpdaterState();
            }
        }

        public async Task SaveLauncherStateAsync(LauncherUpdaterState state)
        {
            await EnsureUpdaterStateFileExistsAsync();
            state.UpdaterStateVersion = LauncherUpdaterState.CurrentUpdaterStateVersion;
            try
            {
                await File.WriteAllTextAsync(UpdaterStateFilePath, JsonConvert.SerializeObject(state));
            }
            catch (IOException)
            {
                return; // TODO: When we add logging, we should log when this exception gets swallowed.
            }
        }

        private async Task EnsureUpdaterStateFileExistsAsync()
        {
            if (!File.Exists(UpdaterStateFilePath))
            {
                await File.WriteAllTextAsync(UpdaterStateFilePath, JsonConvert.SerializeObject(new LauncherUpdaterState()));
            }
        }
    }
}
