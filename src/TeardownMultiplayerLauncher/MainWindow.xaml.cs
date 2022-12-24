using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using TeardownMultiplayerLauncher.Core;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CoreApi _coreApi = new CoreApi();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeLauncherVersionLabel()
        {
            _launcherVersionLabel.Content = $"TDMP Launcher v{_coreApi.GetLauncherVersion()}";
        }

        private void UpdateForm()
        {
            var isGameVersionSupported = _coreApi.HasSupportedTeardownVersion();

            _versionSupportLabel.Content = isGameVersionSupported switch
            {
                true => "TEARDOWN VERSION SUPPORTED",
                false => $"TEARDOWN VERSION UNSUPPORTED (requires {_coreApi.GetSupportedTeardownVersion()})",
                null => "SELECT YOUR TEARDOWN.EXE",
            };
            _versionSupportLabel.Background = isGameVersionSupported switch
            {
                true => new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x88, 0x08)),
                false => new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x00, 0x25)),
                null => new SolidColorBrush(Color.FromArgb(0xFF, 0xA9, 0xA3, 0x00)),
            };

            _teardownFolderTextBox.Content = _coreApi.GetTeardownExePath();

            _playButton.IsEnabled = isGameVersionSupported == true;

            _teardownMultiplayerVersionLabel.Content = $"TDMP v{_coreApi.GetInstalledTeardownMultiplayerVersion()}";
        }

        private async void _teardownFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Please select your teardown.exe";
            dialog.Filter = "teardown.exe|";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await _coreApi.SetTeardownExePathAsync(dialog.FileName);
                UpdateForm();
            }
        }

        private async void _playButton_Click(object sender, RoutedEventArgs e)
        {
            _busyStatusGrid.Visibility = Visibility.Visible;
            try
            {
                if (!Keyboard.IsKeyDown(Key.LeftShift)) // Skip setup if shift is held when clicking Play (primarily for debugging and working around GitHub rate limits).
                {
                    SetBusyStatusText("UPDATING TDMP");
                    await _coreApi.SetUpLatestTeardownMultiplayerReleaseAsync();
                    UpdateForm();
                }

                SetBusyStatusText("GAME RUNNING");
                await _coreApi.LaunchTeardownMultiplayer();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred while launching the game. If the issue persists, please contact support in Discord:\n\n{ex}", "Teardown Multiplayer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _busyStatusGrid.Visibility = Visibility.Hidden;
        }

        private void SetBusyStatusText(string text)
        {
            _busyStatusLabel.Content = text;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _coreApi.InitializeAsync();
            InitializeLauncherVersionLabel();
            UpdateForm();
        }

        private void _discordGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _coreApi.OpenDiscordServer();
        }
    }
}
