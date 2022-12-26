using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Gets the locale data for a specified cultureCode.
        /// If the culture isn't supported, the first available locale data will be provided.
        /// </summary>
        /// <param name="cultureCode">ISO 639-1 two letter culture code.</param>
        /// <returns>LocaleData for specified culture code, or fallback to first available locale data if specified culture is unavailable.</returns>
        public async Task<LocaleData> GetLocaleDataAsync(string cultureCode)
        {
            IEnumerable<string> supportedCultureCodes = GetSupportedCultureCodes();
            if (!supportedCultureCodes.Contains(cultureCode))
            {
                _state.SelectedCultureCode = supportedCultureCodes.First();
                await new LauncherStateRepository().SaveLauncherStateAsync(_state);
                return await _localeDataRepository.GetLocaleDataAsync(supportedCultureCodes.First());
            }
            return await _localeDataRepository.GetLocaleDataAsync(cultureCode);
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
