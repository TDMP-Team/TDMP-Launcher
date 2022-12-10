﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class GameVersionUtility
    {
        public static readonly string SupportedTeardownMd5Hash = "004fb1bf0e06ba9bf2d8eb8d3dc2e142"; // TODO: make configurable or retrieved at runtime

        public static string? GetTeardownMd5Hash(string teardownExePath)
        {
            if (!File.Exists(teardownExePath))
            {
                return null;
            }

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(teardownExePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
                }
            }
        }

        public static bool? HasSupportedTeardownVersion(string teardownExePath)
        {
            var teardownExeMd5Hash = GetTeardownMd5Hash(teardownExePath);
            if (teardownExeMd5Hash == null) // Check if md5 hash can't be calculated, likely due to bad file path.
            {
                return null;
            }
            return string.Equals(teardownExeMd5Hash, SupportedTeardownMd5Hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}