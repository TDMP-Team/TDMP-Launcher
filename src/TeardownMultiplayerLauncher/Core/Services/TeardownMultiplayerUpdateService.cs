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
        private readonly IPackageResolver _packageResolver;
        private readonly TeardownMultiplayerUpdateState _state;

        public TeardownMultiplayerUpdateService(TeardownMultiplayerUpdateState state)
        {
            _packageResolver = new GithubPackageResolver(state.GitHubRepositoryOwner, state.GitHubRepositoryName, state.GitHubAssetFileNamePattern);
            _state = state;
        }

        public async Task SetUpLatestReleaseAsync(string teardownDirectoryPath)
        {
            if (_state.LastCheckDateTimeUtc.HasValue && (DateTime.UtcNow - _state.LastCheckDateTimeUtc) < _state.CheckCooldownDuration) // Prevent frequent release checks/downloads to not get rate limited by GitHub.
            {
                return;
            }

            var latestReleaseVersion = (await _packageResolver.GetPackageVersionsAsync()).First();
            if (!string.Equals(latestReleaseVersion.ToString(), _state.InstalledVersion, StringComparison.Ordinal))
            {
                var zipFilePath = await DownloadReleaseZipAsync(latestReleaseVersion);
                await UninstallCurrentReleaseAsync();
                await InstallLatestReleaseFromZipAsync(zipFilePath, teardownDirectoryPath);
                _state.InstalledVersion = latestReleaseVersion.ToString();
            }
            
            _state.LastCheckDateTimeUtc = DateTime.UtcNow;
        }

        private async Task<string> DownloadReleaseZipAsync(Version releaseVersion)
        {
            var zipFilePath = Path.GetTempFileName();
            await _packageResolver.DownloadPackageAsync(releaseVersion, zipFilePath);
            return zipFilePath;
        }

        private async Task UninstallCurrentReleaseAsync()
        {
            await Task.Run(() =>
            {
                foreach (var filePath in _state.InstalledFilePaths)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                _state.InstalledFilePaths.Clear();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private async Task InstallLatestReleaseFromZipAsync(string zipFilePath, string teardownDirectoryPath)
        {
            await Task.Run(() =>
            {
                using ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath);
                foreach (var zipEntry in zipArchive.Entries)
                {
                    var isFile = !string.IsNullOrWhiteSpace(zipEntry.Name); // Folders don't have "Name" populated, only files do.
                    if (isFile)
                    {
                        _state.InstalledFilePaths.Add(Path.Combine(teardownDirectoryPath, zipEntry.FullName));
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
