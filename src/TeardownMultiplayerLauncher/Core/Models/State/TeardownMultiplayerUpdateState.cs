using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TeardownMultiplayerLauncher.Core.Models.State
{
    internal class TeardownMultiplayerUpdateState
    {
        [JsonProperty("github_repository_owner")]
        public string GitHubRepositoryOwner { get; set; } = "TDMP-Team";

        [JsonProperty("github_repository_name")]
        public string GitHubRepositoryName { get; set; } = "TDMP-Public";

        [JsonProperty("github_asset_file_name_pattern")]
        public string GitHubAssetFileNamePattern { get; set; } = "TDMP*.zip";

        [JsonProperty("last_check_datetime_utc")]
        public DateTime? LastCheckDateTimeUtc { get; set; }

        [JsonProperty("check_cooldown_duration")]
        public TimeSpan CheckCooldownDuration { get; set; } = TimeSpan.FromMinutes(30);

        [JsonProperty("installed_version")]
        public string? InstalledVersion { get; set; }

        [JsonProperty("installed_file_paths")]
        public List<string> InstalledFilePaths { get; set; } = new List<string>();
    }
}
