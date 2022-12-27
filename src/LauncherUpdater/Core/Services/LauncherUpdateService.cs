using Onova.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;
using LauncherUpdater.Core.Utilities;

namespace LauncherUpdater.Core.Services
{
    internal class LauncherUpdateService
    {
        private event Action<double> DownloadProgress;

        private readonly LauncherUpdaterState _state;
        private readonly IPackageResolver _packageResolver;

        public LauncherUpdateService(LauncherUpdaterState state)
        {
            _state = state;
            _packageResolver = new GithubPackageResolver(
                state.GitHubRepositoryOwner,
                state.GitHubRepositoryName,
                state.GitHubAssetFileNamePattern
            );
            DownloadProgress += DownloadProgressChanged;
        }

        public async Task SetUpLatestReleaseAsync()
        {
            if (IsOnCooldown())
            {
                return;
            }

            var availablePackageVersions = await _packageResolver.GetPackageVersionsAsync();
            var latestReleaseVersion = availablePackageVersions.First(); // Assume first package version is latest.
            _state.LastCheckDateTimeUtc = DateTime.UtcNow;

            if (IsLatestReleaseNewerThanInstalledVersion(latestReleaseVersion))
            {
                var zipFilePath = await DownloadReleaseZipAsync(latestReleaseVersion);
                UninstallCurrentRelease();
                await InstallLatestReleaseFromZipAsync(zipFilePath);
                _state.InstalledVersion = latestReleaseVersion.ToString();
            }
        }

        private void DownloadProgressChanged(double newValue)
        {
            _state.Progress = (float)(newValue * 100.0f);
            _state.CurrentTask = "Updating...";
        }

        /// <summary>
        /// Prevent frequent release checks/downloads to avoid getting rate-limited by GitHub.
        /// </summary>
        private bool IsOnCooldown()
        {
            return _state.LastCheckDateTimeUtc.HasValue && (DateTime.UtcNow - _state.LastCheckDateTimeUtc) < _state.CheckCooldownDuration;
        }

        private bool IsLatestReleaseNewerThanInstalledVersion(Version latestReleaseVersion)
        {
            return !string.Equals(latestReleaseVersion.ToString(), _state.InstalledVersion, StringComparison.Ordinal);
        }

        private async Task<string> DownloadReleaseZipAsync(Version releaseVersion)
        {
            var zipFilePath = Path.GetTempFileName();
            Progress<double> downloadProgress = new(DownloadProgress);
            await _packageResolver.DownloadPackageAsync(releaseVersion, zipFilePath, downloadProgress);
            return zipFilePath;
        }

        private void UninstallCurrentRelease()
        {
            if (Directory.Exists(PathUtility.TeardownLauncherDirectory))
            {
                Directory.Delete(PathUtility.TeardownLauncherDirectory, true);
            }
        }

        private Task InstallLatestReleaseFromZipAsync(string zipFilePath)
        {
            return Task.Run(() =>
            {
                using var zipArchive = ZipFile.OpenRead(zipFilePath);
                ZipFile.ExtractToDirectory(zipFilePath, PathUtility.TeardownLauncherDirectory, true);
                var extractedDirectory = Directory.EnumerateDirectories(PathUtility.TeardownLauncherDirectory).First(); // Gets the TDMP-Launcher-x.x.x directory that was extracted from the zip.
                FileUtility.MoveDirectoryContents(extractedDirectory, PathUtility.TeardownLauncherDirectory);
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }
    }
}
