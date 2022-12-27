using System;
using System.Linq;
using System.Threading;
using System.Windows;
using TeardownMultiplayerLauncher.Core;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#pragma warning disable IDE0052 // Remove unread private members
        private static Mutex? Mutex;
#pragma warning restore IDE0052 // Remove unread private members

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EnsureOnlySingleInstanceRunning();
            var coreApi = await CoreApi.CreateCoreApiAsync();
            if (e.Args.Any())
            {
                await new CoreApiCommandLineExecutor(coreApi).ExecuteAsync(e.Args);
                Shutdown();
            }
            else
            {
                new MainWindow(coreApi).Show();
            }
        }

        private void EnsureOnlySingleInstanceRunning()
        {
            Mutex = new Mutex(true, "TDMPLauncher", out var isNewMutex);
            if (!isNewMutex)
            {
                MessageBox.Show("Launcher is already running.", "Teardown Multiplayer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
            }
        }
    }
}
