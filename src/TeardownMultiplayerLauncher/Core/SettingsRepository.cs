using Newtonsoft.Json;
using System.IO;

namespace TeardownMultiplayerLauncher.Core
{
    internal class SettingsRepository
    {
        private static readonly string SettingsFilePath = "settings.json";

        public Settings GetSettings()
        {
            EnsureSettingsFileExists();
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilePath));
        }

        public void SaveSettings(Settings settings)
        {
            EnsureSettingsFileExists();
            File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(settings));
        }

        private void EnsureSettingsFileExists()
        {
            if (!File.Exists(SettingsFilePath))
            {
                File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(new Settings()));
            }
        }
    }
}
