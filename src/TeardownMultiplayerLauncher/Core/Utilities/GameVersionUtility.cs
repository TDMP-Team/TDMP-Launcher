using System;
using System.IO;
using System.Security.Cryptography;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class GameVersionUtility
    {
        public static readonly string SupportedTeardownVersion = "1.4.0";
        public static readonly string SupportedTeardownExeMd5Hash = "92f62a1c46c45eb29c949af0262dcfcb";

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
            return string.Equals(teardownExeMd5Hash, SupportedTeardownExeMd5Hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
