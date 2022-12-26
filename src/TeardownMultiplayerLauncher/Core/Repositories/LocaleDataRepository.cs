using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using TeardownMultiplayerLauncher.Core.Models;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class LocaleDataRepository
    {
        public static readonly string LocalesDirectory = "Locales/";
        private static bool shownLocaleError = false;

        public async Task<LocaleData> GetLocaleDataAsync(string cultureCode)
        {
            var localeDataFilePath = Path.Combine(LocalesDirectory, $"{cultureCode}.json");
            await EnsureLocaleDataFileExistsAsync(localeDataFilePath);
            try
            {
                return JsonConvert.DeserializeObject<LocaleData>(
                    await File.ReadAllTextAsync(localeDataFilePath)
                );
            } catch (JsonReaderException)
            {
                if(!shownLocaleError) { 
                    MessageBox.Show($"A severe error was detected in the {cultureCode}.json locale file. Please reinstall your launcher.", "Teardown Multiplayer", MessageBoxButton.OK, MessageBoxImage.Error);
                    shownLocaleError = true;
                }
                return new LocaleData();
            }
        }

        private Task EnsureLocaleDataFileExistsAsync(string localeStringsFilePath)
        {
            return FileUtility.CreateDirectoriesAndFileAsync(localeStringsFilePath, JsonConvert.SerializeObject(new LocaleData()), false);
        }
    }
}
