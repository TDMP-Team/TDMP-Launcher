using CommandLine;
using CommandLine.Text;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace TeardownMultiplayerLauncher.Core
{
    internal class CoreApiCommandLineExecutor
    {
        private class Options
        {
            [Option("play", Required = false, HelpText = "Launches Teardown immediately, skipping the launcher UI.")]
            public bool ShouldPlayImmediately { get; set; }
        }

        private readonly CoreApi _coreApi;

        public CoreApiCommandLineExecutor(CoreApi coreApi)
        {
            _coreApi = coreApi;
        }

        public Task ExecuteAsync(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<Options>(args);
            return parseResult.WithParsedAsync(async options =>
            {
                if (options.ShouldPlayImmediately)
                {
                    await _coreApi.LaunchTeardownMultiplayerAsync();
                }
            });
        }
    }
}