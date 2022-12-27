using Newtonsoft.Json;

namespace TeardownMultiplayerLauncher.Core.Models.Localization
{
    public class LocaleStrings
    {
        [JsonProperty("title")]
        public string LauncherTitle { get; set; } = string.Empty;

        [JsonProperty("join_discord")]
        public string JoinDiscordText { get; set; } = string.Empty;

        [JsonProperty("teardown_path")]
        public string TeardownPathText { get; set; } = string.Empty;

        [JsonProperty("select_teardown")]
        public string SelectTeardownText { get; set; } = string.Empty;

        [JsonProperty("teardown_invalid")]
        public string TeardownInvalidText { get; set; } = string.Empty;

        [JsonProperty("teardown_valid")]
        public string TeardownValidText { get; set; } = string.Empty;

        [JsonProperty("select_teardown_dialog")]
        public string SelectDialogText { get; set; } = string.Empty;

        [JsonProperty("injection_delay_title")]
        public string InjectionTitleText { get; set; } = string.Empty;

        [JsonProperty("injection_delay_description")]
        public string InjectionDescriptionText { get; set; } = string.Empty;

        [JsonProperty("seconds")]
        public string SecondsText { get; set; } = string.Empty;

        [JsonProperty("play_button")]
        public string PlayButtonText { get; set; } = string.Empty;

        [JsonProperty("updating_tdmp")]
        public string UpdatingTDMPText { get; set; } = string.Empty;

        [JsonProperty("game_running")]
        public string GameRunningText { get; set; } = string.Empty;

        [JsonProperty("launch_error")]
        public string LaunchErrorText { get; set; } = string.Empty;

        [JsonProperty("launcher_version")]
        public string LauncherVersionText { get; set; } = string.Empty;

        [JsonProperty("view_release_notes")]
        public string ViewReleaseNotesText { get; set; } = string.Empty;

        [JsonProperty("browse")]
        public string BrowseText { get; set; } = string.Empty;
    }
}
