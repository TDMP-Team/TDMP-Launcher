using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class LanguageRepository
    {
        private static readonly string LanguageFilePath = "lang/lang-{0}.json";

        public static async Task<LanguageTranslations> GetLanguageAsync(string languageCode)
        {
            await EnsureLanguageFileExistsAsync(languageCode);
            var languageTranslations = JsonConvert.DeserializeObject<LanguageTranslations>(await File.ReadAllTextAsync(string.Format(LanguageFilePath, languageCode)));
            return languageTranslations;
        }

        private static async Task EnsureLanguageFileExistsAsync(string languageCode)
        {
            if (!Directory.Exists("lang"))
            {
                Directory.CreateDirectory("lang");
            }
            if (!File.Exists(string.Format(LanguageFilePath, languageCode)))
            {
                await File.WriteAllTextAsync(string.Format(LanguageFilePath, languageCode), JsonConvert.SerializeObject(new LanguageTranslations()));
            }
        }
    }
}
