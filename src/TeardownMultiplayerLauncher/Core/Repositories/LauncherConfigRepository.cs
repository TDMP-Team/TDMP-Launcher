using Newtonsoft.Json;
using System.IO;
using TeardownMultiplayerLauncher.Core.Models;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class LauncherConfigRepository
    {
        private static readonly string LauncherConfigFilePath = "config.json";

        public LauncherConfig GetLauncherConfig()
        {
            EnsureLauncherConfigFileExists();
            return JsonConvert.DeserializeObject<LauncherConfig>(File.ReadAllText(LauncherConfigFilePath));
        }

        public void SaveLauncherConfig(LauncherConfig config)
        {
            EnsureLauncherConfigFileExists();
            File.WriteAllText(LauncherConfigFilePath, JsonConvert.SerializeObject(config));
        }

        private void EnsureLauncherConfigFileExists()
        {
            if (!File.Exists(LauncherConfigFilePath))
            {
                File.WriteAllText(LauncherConfigFilePath, JsonConvert.SerializeObject(new LauncherConfig()));
            }
        }
    }
}
