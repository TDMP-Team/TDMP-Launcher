using System.Threading;
using System.Windows;

namespace LauncherUpdater
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
            _mutex = new Mutex(true, "TDMPLauncherUpdater", out var isNewMutex);
            if (!isNewMutex)
            {
                MessageBox.Show("Updater is already running.", "TDMP Updater", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown();
            }
        }
    }
}
