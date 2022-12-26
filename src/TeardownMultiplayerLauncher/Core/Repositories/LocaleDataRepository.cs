using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TeardownMultiplayerLauncher.Core.Models;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class LocaleDataRepository
    {
        public static readonly string LocalesDirectory = "Locales/";

        public async Task<LocaleData> GetLocaleDataAsync(string cultureCode)
        {
            var localeDataFilePath = Path.Combine(LocalesDirectory, $"{cultureCode}.json");
            await EnsureLocaleDataFileExistsAsync(localeDataFilePath);
            return JsonConvert.DeserializeObject<LocaleData>(
                await File.ReadAllTextAsync(localeDataFilePath)
            );
        }

        private Task EnsureLocaleDataFileExistsAsync(string localeStringsFilePath)
        {
            return FileUtility.CreateDirectoriesAndFileAsync(localeStringsFilePath, JsonConvert.SerializeObject(new LocaleData()), false);
        }
    }
}
