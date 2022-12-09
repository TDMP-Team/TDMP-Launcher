using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core.Models
{
    internal class State
    {
        [JsonProperty("teardown_exe_path")]
        public string TeardownExePath { get; set; }
    }
}
