using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core.Models.State
{
    internal class LauncherState
    {
        public static readonly int CurrentLauncherStateVersion = 1; // Bump this whenever a contract-breaking change is made to the state model. This will ensure incompatible states are reset.

        [JsonProperty("launcher_state_version")]
        public int LauncherStateVersion { get; set; }

        [JsonProperty("teardown_exe_path")]
        public string? TeardownExePath { get; set; }

        [JsonProperty("teardown_multiplayer_update_state")]
        public TeardownMultiplayerUpdateState TeardownMultiplayerUpdateState { get; set; } = new TeardownMultiplayerUpdateState();
    }
}
