using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using TeardownMultiplayerLauncher.Core;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Core.Core _core = new Core.Core();

        public MainWindow()
        {
            InitializeComponent();
            InitializeLauncherVersionLabel();
        }

        private void InitializeLauncherVersionLabel()
        {
            _launcherVersionLabel.Content = $"TDMP Launcher v{_core.GetLauncherVersion()}";
        }

        private void UpdateForm()
        {
            var isGameVersionSupported = _core.HasSupportedTeardownVersion();

            _versionSupportLabel.Content = isGameVersionSupported switch
            {
                true => "TEARDOWN VERSION SUPPORTED",
                false => "TEARDOWN VERSION UNSUPPORTED",
                null => "SELECT YOUR TEARDOWN.EXE",
            };
            _versionSupportLabel.Background = isGameVersionSupported switch
            {
                true => new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x88, 0x08)),
                false => new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x00, 0x25)),
                null => new SolidColorBrush(Color.FromArgb(0xFF, 0xA9, 0xA3, 0x00)),
            };

            _playButton.IsEnabled = isGameVersionSupported == true;
        }

        private void _teardownFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Please select your teardown.exe";
            dialog.Filter = "teardown.exe|";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _core.SetTeardownExePath(dialog.FileName);
                _teardownFolderTextBox.Content = dialog.FileName;
                UpdateForm();
            }
        }

        private void _playButton_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => {
                if (!_core.LaunchTeardownMultiplayer())
                {
                    System.Windows.MessageBox.Show("Failed to inject TDMP, please try again. If issue persists, please contact support in Discord.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).Start();
        }
    }
}
