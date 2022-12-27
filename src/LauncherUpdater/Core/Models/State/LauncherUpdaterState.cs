using Newtonsoft.Json;
using System;

namespace LauncherUpdater.Core.Models.State
{
    internal class LauncherUpdaterState
    {
        public static readonly int CurrentUpdaterStateVersion = 1; // Bump this whenever a contract-breaking change is made to the state model. This will ensure incompatible states are reset.

        [JsonProperty("updater_state_version")]
        public int UpdaterStateVersion { get; set; }

        [JsonProperty("github_repository_owner")]
        public string GitHubRepositoryOwner { get; set; } = "TDMP-Team";

        [JsonProperty("github_repository_name")]
        public string GitHubRepositoryName { get; set; } = "TDMP-Launcher-Public";

        [JsonProperty("github_asset_file_name_pattern")]
        public string GitHubAssetFileNamePattern { get; set; } = "TDMP-Launcher-*.zip";

        [JsonProperty("last_check_datetime_utc")]
        public DateTime? LastCheckDateTimeUtc { get; set; }

        [JsonProperty("check_cooldown_duration")]
        public TimeSpan CheckCooldownDuration { get; set; } = TimeSpan.FromMinutes(30);

        [JsonProperty("installed_version")]
        public string? InstalledVersion { get; set; }

        [JsonIgnore]
        public float Progress { get; set; } = 0;

        [JsonIgnore]
        public string CurrentTask { get; set; } = "Checking For Updates...";
    }
}
