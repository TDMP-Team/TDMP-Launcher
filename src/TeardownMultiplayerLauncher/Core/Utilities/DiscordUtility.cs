using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TeardownMultiplayerLauncher.Core.Utilities;
internal static class DiscordUtility
{
    private static readonly HashSet<string> DiscordClientDirectories = new HashSet<string> { "Discord", "DiscordCanary", "DiscordPTB" };

    /// <summary>
    /// Opens the specified Discord server in a web browser or in a Discord client if detected on the system.
    /// </summary>
    /// <param name="serverUrl">Discord server URL</param>
    public static void OpenDiscordServer(string serverUrl)
    {
        Process.Start(
            new ProcessStartInfo(IsDiscordClientInstalled() ? $"discord:{serverUrl}" : serverUrl)
            {
                UseShellExecute = true,
                Verb = "open",
            }
        );
    }

    private static bool IsDiscordClientInstalled()
    {
        var appData = Environment.ExpandEnvironmentVariables("%localappdata%");
        return DiscordClientDirectories.Any(discordDirectory =>
        {
            var directory = Path.Combine(appData, discordDirectory);
            return
                Directory.Exists(directory) &&
                !File.Exists(Path.Combine(directory, ".dead")); // If Discord is uninstalled, it leaves a .dead file in the directory to indicate this.
        });
    }
}
