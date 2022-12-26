using Onova.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using TeardownMultiplayerLauncher.Core.Models.State;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class TeardownMultiplayerUpdateService
    {
        private readonly LauncherState _state;
        private readonly IPackageResolver _packageResolver;

        public TeardownMultiplayerUpdateService(LauncherState state)
        {
            _state = state;
            _packageResolver = new GithubPackageResolver(
                state.TeardownMultiplayerUpdateState.GitHubRepositoryOwner,
                state.TeardownMultiplayerUpdateState.GitHubRepositoryName,
                state.TeardownMultiplayerUpdateState.GitHubAssetFileNamePattern
            );
        }

        public async Task SetUpLatestReleaseAsync(string teardownDirectory)
        {
            if (IsOnCooldown())
            {
                return;
            }

            var availablePackageVersions = await _packageResolver.GetPackageVersionsAsync();
            var latestReleaseVersion = availablePackageVersions.First(); // Assume first package version is latest.
            if (IsLatestReleaseNewerThanInstalledVersion(latestReleaseVersion))
            {
                var zipFilePath = await DownloadReleaseZipAsync(latestReleaseVersion);
                await UninstallCurrentReleaseAsync(teardownDirectory);
                await InstallLatestReleaseFromZipAsync(zipFilePath, teardownDirectory);
                _state.TeardownMultiplayerUpdateState.InstalledVersion = latestReleaseVersion.ToString();
            }

            _state.TeardownMultiplayerUpdateState.LastCheckDateTimeUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Prevent frequent release checks/downloads to avoid getting rate-limited by GitHub.
        /// </summary>
        private bool IsOnCooldown()
        {
            return _state.TeardownMultiplayerUpdateState.LastCheckDateTimeUtc.HasValue && (DateTime.UtcNow - _state.TeardownMultiplayerUpdateState.LastCheckDateTimeUtc) < _state.TeardownMultiplayerUpdateState.CheckCooldownDuration;
        }

        private bool IsLatestReleaseNewerThanInstalledVersion(Version latestReleaseVersion)
        {
            return !string.Equals(latestReleaseVersion.ToString(), _state.TeardownMultiplayerUpdateState.InstalledVersion, StringComparison.Ordinal);
        }

        private async Task<string> DownloadReleaseZipAsync(Version releaseVersion)
        {
            var zipFilePath = Path.GetTempFileName();
            await _packageResolver.DownloadPackageAsync(releaseVersion, zipFilePath);
            return zipFilePath;
        }

        private Task UninstallCurrentReleaseAsync(string teardownDirectory)
        {
            return Task.Run(() =>
            {
                // Remove TDMP folders. This is required in case if files was removed or something unexpected happened.
                var tdmpModsDirectory = Path.Combine(teardownDirectory, "mods/TDMP");
                var tdmpDataDirectory = Path.Combine(teardownDirectory, "data/TDMP");
                if (Directory.Exists(tdmpModsDirectory))
                {
                    Directory.Delete(tdmpModsDirectory, true);
                }
                if (Directory.Exists(tdmpDataDirectory))
                {
                    Directory.Delete(tdmpDataDirectory, true);
                }

                foreach (var filePath in _state.TeardownMultiplayerUpdateState.InstalledFilePaths)
                {
                    if (File.Exists(filePath) && filePath.StartsWith(teardownDirectory))
                    {
                        File.Delete(filePath);
                    }
                }
                _state.TeardownMultiplayerUpdateState.InstalledFilePaths.Clear();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private Task InstallLatestReleaseFromZipAsync(string zipFilePath, string teardownDirectoryPath)
        {
            return Task.Run(() =>
            {
                using ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath);
                foreach (var zipEntry in zipArchive.Entries)
                {
                    var isFile = !string.IsNullOrWhiteSpace(zipEntry.Name); // Folders don't have "Name" populated, only files do.
                    if (isFile)
                    {
                        _state.TeardownMultiplayerUpdateState.InstalledFilePaths.Add(Path.Combine(teardownDirectoryPath, zipEntry.FullName));
                    }
                }
                ZipFile.ExtractToDirectory(zipFilePath, teardownDirectoryPath, true);
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
