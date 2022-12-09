using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core
{
    internal class Settings
    {
        [JsonProperty("teardown_exe_path")]
        public string TeardownExePath { get; set; }
    }
}
