using CommandLine;
using System;
using System.Linq;
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

        public async Task ExecuteAsync(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<Options>(args);
            await parseResult.WithParsedAsync(async options =>
            {
                if (options.ShouldPlayImmediately)
                {
                    await _coreApi.LaunchTeardownMultiplayerAsync();
                    Environment.Exit(0);
                }
            });
            parseResult.WithNotParsed(errors =>
            {
                MessageBox.Show($"Some or all launch parameters provided failed to parse:\n\n{string.Join("\n", errors.Select(error => error.ToString()))}");
            });
        }
    }
}