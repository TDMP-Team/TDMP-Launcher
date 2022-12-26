using Newtonsoft.Json;
using TeardownMultiplayerLauncher.Core.Models.Localization;

namespace TeardownMultiplayerLauncher.Core.Models
{
    internal class LocaleData
    {
        // New locale-specific data such as images/icons, etc can be added here.

        /// <summary>
        /// ISO 639-1 two letter culture code.
        /// </summary>
        [JsonProperty("culture_code")]
        public string CultureCode { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("strings")]
        public LocaleStrings Strings { get; set; } = new LocaleStrings();
    }
}
