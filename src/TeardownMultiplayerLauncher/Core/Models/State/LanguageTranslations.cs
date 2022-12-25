using Newtonsoft.Json;
using System;

namespace TeardownMultiplayerLauncher.Core.Models.State
{
    internal class LanguageTranslations
    {
        [JsonProperty("title")]
        public string LauncherTitle { get; set; } = "";

        [JsonProperty("join_discord")]
        public string JoinDiscordText { get; set; } = "";

        [JsonProperty("teardown_path")]
        public string TeardownPathText { get; set; } = "";

        [JsonProperty("select_teardown")]
        public string SelectTeardownText { get; set; } = "";

        [JsonProperty("teardown_invalid")]
        public string TeardownInvalidText { get; set; } = "";

        [JsonProperty("teardown_valid")]
        public string TeardownValidText { get; set; } = "";

        [JsonProperty("select_teardown_dialog")]
        public string SelectDialogText { get; set; } = "";

        [JsonProperty("injection_delay_title")]
        public string InjectionTitleText { get; set; } = "";

        [JsonProperty("injection_delay_description")]
        public string InjectionDescriptionText { get; set; } = "";

        [JsonProperty("seconds")]
        public string SecondsText { get; set; } = "";

        [JsonProperty("play_button")]
        public string PlayButtonText { get; set; } = "";

        [JsonProperty("updating_tdmp")]
        public string UpdatingTDMPText { get; set; } = "";

        [JsonProperty("game_running")]
        public string GameRunningText { get; set; } = "";

        [JsonProperty("launch_error")]
        public string LaunchErrorText { get; set; } = "";

        [JsonProperty("launcher_version")]
        public string LauncherVersionText { get; set; } = "";

        [JsonProperty("browse")]
        public string BrowseText { get; set; } = "";

        [JsonProperty("language_english")]
        public string EnglishLanguageText { get; set; } = "";

        [JsonProperty("language_russian")]
        public string RussianLanguageText { get; set; } = "";
    }
}
