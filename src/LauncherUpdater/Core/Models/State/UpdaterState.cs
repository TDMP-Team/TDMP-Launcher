using Newtonsoft.Json;
using System;
using System.Globalization;

namespace LauncherUpdater.Core.Models.State
{
    internal class UpdaterState
    {
        public static readonly int CurrentLauncherStateVersion = 1; // Bump this whenever a contract-breaking change is made to the state model. This will ensure incompatible states are reset.

        [JsonProperty("launcher_state_version")]
        public int LauncherStateVersion { get; set; }

        [JsonProperty("discord_url")]
        public string DiscordUrl { get; set; } = "https://discord.gg/h8eSabqdA6";

        [JsonProperty("injection_delay")]
        public TimeSpan InjectionDelay { get; set; } = TimeSpan.FromSeconds(3);

        [JsonProperty("launcher_exe_path")]
        public string? LauncherExePath { get; set; }

        [JsonProperty("teardown_multiplayer_update_state")]
        public LauncherUpdateState LauncherUpdateState { get; set; } = new LauncherUpdateState();

        /// <summary>
        /// ISO 639-1 two letter culture code.
        /// </summary>
        [JsonProperty("selected_culture_code")]
        public string SelectedCultureCode { get; set; } = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        [JsonProperty("percentage_done")]
        public float PercentageDone = 0;

        [JsonProperty("current_task")]
        public string CurrentTask = "Checking For Updates...";
    }
}
