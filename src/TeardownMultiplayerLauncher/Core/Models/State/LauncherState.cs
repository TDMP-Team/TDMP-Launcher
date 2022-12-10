using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core.Models.State
{
    internal class LauncherState
    {
        [JsonProperty("teardown_exe_path")]
        public string TeardownExePath { get; set; }

        [JsonProperty("teardown_multiplayer_update_state")]
        public TeardownMultiplayerUpdateState TeardownMultiplayerUpdateState { get; set; } = new TeardownMultiplayerUpdateState();
    }
}
