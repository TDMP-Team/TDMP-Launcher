using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core.Models
{
    internal class LauncherConfig
    {
        [JsonProperty("teardown_exe_path")]
        public string TeardownExePath { get; set; }

        [JsonProperty("teardown_multiplayer_release_github_api_url")]
        public string TeardownMultiplayerReleaseGitHubApiUrl { get; set; } = "https://api.github.com/repos/DangerKiddy/TDMP-Public/releases";
    }
}
