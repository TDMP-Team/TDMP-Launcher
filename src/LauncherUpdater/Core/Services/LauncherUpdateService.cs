using Onova.Services;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using LauncherUpdater.Core.Models.State;
using System.Threading;
using System.Diagnostics;

namespace LauncherUpdater.Core.Services
{
    internal class LauncherUpdateService
    {
        private readonly UpdaterState _state;
        private readonly IPackageResolver _packageResolver;
        private event Action<double> DownloadProgress;

        public LauncherUpdateService(UpdaterState state)
        {
            _state = state;
            _packageResolver = new GithubPackageResolver(
                state.LauncherUpdateState.GitHubRepositoryOwner,
                state.LauncherUpdateState.GitHubRepositoryName,
                state.LauncherUpdateState.GitHubAssetFileNamePattern
            );
            DownloadProgress += DownloadProgressChanged;
        }

        public async Task SetUpLatestReleaseAsync(string launcherDirectory)
        {
            if (IsOnCooldown())
            {
                return;
            }

            var availablePackageVersions = await _packageResolver.GetPackageVersionsAsync();
            var latestReleaseVersion = availablePackageVersions.First(); // Assume first package version is latest.
            _state.LauncherUpdateState.LastCheckDateTimeUtc = DateTime.UtcNow;

            if (IsLatestReleaseNewerThanInstalledVersion(latestReleaseVersion))
            {
                var zipFilePath = await DownloadReleaseZipAsync(latestReleaseVersion);
                await UninstallCurrentReleaseAsync(launcherDirectory);
                await InstallLatestReleaseFromZipAsync(zipFilePath, launcherDirectory);
                _state.LauncherUpdateState.InstalledVersion = latestReleaseVersion.ToString();

            }
        }

        private void DownloadProgressChanged(double newValue)
        {
            _state.PercentageDone = (float)(newValue*100);
            _state.CurrentTask = "Updating...";
        }

        /// <summary>
        /// Prevent frequent release checks/downloads to avoid getting rate-limited by GitHub.
        /// </summary>
        private bool IsOnCooldown()
        {
            return _state.LauncherUpdateState.LastCheckDateTimeUtc.HasValue && (DateTime.UtcNow - _state.LauncherUpdateState.LastCheckDateTimeUtc) < _state.LauncherUpdateState.CheckCooldownDuration;
        }

        private bool IsLatestReleaseNewerThanInstalledVersion(Version latestReleaseVersion)
        {
            return !string.Equals(latestReleaseVersion.ToString(), _state.LauncherUpdateState.InstalledVersion, StringComparison.Ordinal);
        }

        private async Task<string> DownloadReleaseZipAsync(Version releaseVersion)
        {
            var zipFilePath = Path.GetTempFileName();
            Progress<double> downloadProgress = new(DownloadProgress);
            await _packageResolver.DownloadPackageAsync(releaseVersion, zipFilePath, downloadProgress);
            return zipFilePath;
        }

        private void CopyDirectoryContents(string sourcePath, string destinationPath, bool overwriteFiles = true)
        {
            var allDirectories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);

            foreach (string dir in allDirectories)
            {
                string dirToCreate = dir.Replace(sourcePath, destinationPath);
                Directory.CreateDirectory(dirToCreate);
            }
            var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

            foreach (string newPath in allFiles)
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), overwriteFiles);
                File.Delete(newPath);
            }
        }

        private Task UninstallCurrentReleaseAsync(string launcherDirectory)
        {
            return Task.Run(() =>
            {
                // Remove TDMP folders. This is required in case if files was removed or something unexpected happened.
                if (Directory.Exists(launcherDirectory))
                {
                    Directory.Delete(launcherDirectory, true);
                }

                foreach (var filePath in _state.LauncherUpdateState.InstalledFilePaths)
                {
                    if (File.Exists(filePath) && filePath.StartsWith(launcherDirectory))
                    {
                        File.Delete(filePath);
                    }
                }
                _state.LauncherUpdateState.InstalledFilePaths.Clear();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private Task InstallLatestReleaseFromZipAsync(string zipFilePath, string launcherDirectoryPath)
        {
            return Task.Run(() =>
            {
                using ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath);
                foreach (var zipEntry in zipArchive.Entries)
                {
                    var isFile = !string.IsNullOrWhiteSpace(zipEntry.Name); // Folders don't have "Name" populated, only files do.
                    if (isFile)
                    {
                        _state.LauncherUpdateState.InstalledFilePaths.Add(Path.Combine(launcherDirectoryPath, zipEntry.FullName));
                    }
                }
                ZipFile.ExtractToDirectory(zipFilePath, launcherDirectoryPath, true);
                string unZipDir = Directory.EnumerateDirectories(launcherDirectoryPath).First();
                CopyDirectoryContents(unZipDir, launcherDirectoryPath);
                Directory.Delete(unZipDir, true);
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
