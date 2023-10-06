using System.Runtime.InteropServices;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    internal static class TeardownMultiplayerInjectionUtility
    {
        private const string InjectorPath = "TeardownMultiplayerInjector.dll";

        [DllImport(InjectorPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LaunchAndInjectAndWaitForGameToClose(string teardownDirectory);
    }
}
