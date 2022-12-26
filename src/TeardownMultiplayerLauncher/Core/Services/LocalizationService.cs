using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Repositories;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class LocalizationService
    {
        private readonly LauncherState _state;
        private readonly LocaleDataRepository _localeDataRepository;

        public LocalizationService(LauncherState state)
        {
            _state = state;
            _localeDataRepository = new LocaleDataRepository();
        }

        public Task<LocaleData> GetLocaleDataAsync(string cultureCode)
        {
            return _localeDataRepository.GetLocaleDataAsync(cultureCode);
        }

        /// <summary>
        /// Returns supported culture codes by getting all file names listed in the locales directory.
        /// </summary>
        public IEnumerable<string> GetSupportedCultureCodes()
        {
            foreach (var filePath in Directory.GetFiles(LocaleDataRepository.LocalesDirectory))
            {
                yield return Path.GetFileNameWithoutExtension(filePath); // File name should be an ISO 639-1 two letter culture code.
            }
        }
    }
}
