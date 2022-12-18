using System.Threading;
using System.Windows;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EnsureOnlySingleInstanceRunning();
        }

        private void EnsureOnlySingleInstanceRunning()
        {
            _mutex = new Mutex(true, "TDMPLauncher", out var isNewMutex);
            if (!isNewMutex)
            {
                MessageBox.Show("Launcher is already running.", "Teardown Multiplayer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
            }
        }
    }
}
