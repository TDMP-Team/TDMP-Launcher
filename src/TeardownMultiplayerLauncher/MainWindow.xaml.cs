using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using TeardownMultiplayerLauncher.Core;
using TeardownMultiplayerLauncher.Core.Models;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CoreApi _coreApi;
        private LocaleData _currentLocaleData;

        public MainWindow(CoreApi coreApi)
        {
            _coreApi = coreApi;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeLocaleComboBoxAsync();
            await InitializeLocaleDataAsync(_coreApi.GetSelectedCultureCode());
            UpdateForm();
        }

        private void UpdateForm()
        {
            var isGameVersionSupported = _coreApi.HasSupportedTeardownVersion();

            _versionSupportLabel.Content = isGameVersionSupported switch
            {
                true => _currentLocaleData.Strings.TeardownValidText,
                false => string.Format(_currentLocaleData.Strings.TeardownInvalidText, _coreApi.GetSupportedTeardownVersion()),
                null => _currentLocaleData.Strings.SelectTeardownText,
            };
            _versionSupportLabel.Background = isGameVersionSupported switch
            {
                true => new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x88, 0x08)),
                false => new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x00, 0x25)),
                null => new SolidColorBrush(Color.FromArgb(0xFF, 0xA9, 0xA3, 0x00)),
            };

            _launcherMainWindow.Title = _currentLocaleData.Strings.LauncherTitle;

            _joinDiscordLabel.Content = _currentLocaleData.Strings.JoinDiscordText;

            _teardownPathLabel.Content = _currentLocaleData.Strings.TeardownPathText;
            _teardownFolderBrowseButton.Content = _currentLocaleData.Strings.BrowseText;
            _teardownFolderTextBox.Content = _coreApi.GetTeardownExePath();

            _injectionTitleLabel.Content = _currentLocaleData.Strings.InjectionTitleText;
            _injectionDescriptionLabel.Content = _currentLocaleData.Strings.InjectionDescriptionText;
            _injectionDelaySlider.Value = _coreApi.GetInjectionDelay().TotalSeconds;
            _injectionDelayLabel.Content = string.Format(_currentLocaleData.Strings.SecondsText, _coreApi.GetInjectionDelay().TotalSeconds);

            _playButton.Content = _currentLocaleData.Strings.PlayButtonText;
            _playButton.IsEnabled = isGameVersionSupported == true;

            _teardownMultiplayerVersionLabel.Content = $"TDMP v{_coreApi.GetInstalledTeardownMultiplayerVersion()}";
            _launcherVersionLabel.Content = string.Format(_currentLocaleData.Strings.LauncherVersionText, _coreApi.GetLauncherVersion());
        }

        private void SetBusyStatusText(string text)
        {
            _busyStatusLabel.Content = text;
        }

        private async Task InitializeLocaleComboBoxAsync()
        {
            var index = 0;
            foreach (var cultureCode in _coreApi.GetSupportedCultureCodes())
            {
                var localeData = await _coreApi.GetLocaleDataAsync(cultureCode);
                _localeComboBox.Items.Insert(index++, new ComboBoxItem { Name = cultureCode, Content = localeData.Name });
            }
        }

        private async Task InitializeLocaleDataAsync(string? cultureCode = null)
        {
            cultureCode ??= _coreApi.GetSelectedCultureCode();
            await _coreApi.SetSelectedCultureCodeAsync(cultureCode);
            _currentLocaleData = await _coreApi.GetLocaleDataAsync(cultureCode);

            if(!_coreApi.GetSupportedCultureCodes().Contains(cultureCode))
            {
                _localeComboBox.SelectedIndex = 0;
                return;
            }

            for (var index = 0; index < _localeComboBox.Items.Count; ++index)
            {
                var comboBoxItem = (ComboBoxItem)_localeComboBox.Items.GetItemAt(index);
                if (comboBoxItem.Name == cultureCode)
                {
                    _localeComboBox.SelectedIndex = index;
                    break;
                }
            }
        }

        private async void _teardownFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = _currentLocaleData.Strings.SelectDialogText;
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
                    SetBusyStatusText(_currentLocaleData.Strings.UpdatingTDMPText);
                    await _coreApi.SetUpLatestTeardownMultiplayerReleaseAsync();
                    UpdateForm();
                }

                SetBusyStatusText(_currentLocaleData.Strings.GameRunningText);
                await _coreApi.LaunchTeardownMultiplayerAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format(_currentLocaleData.Strings.LaunchErrorText, ex), _currentLocaleData.Strings.LauncherTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _busyStatusGrid.Visibility = Visibility.Hidden;
        }

        private void _discordGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _coreApi.OpenDiscordServer();
        }

        private async void _injectionDelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            await _coreApi.SetInjectionDelayAsync(TimeSpan.FromSeconds(e.NewValue));
            UpdateForm();
        }

        private async void _localeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxItem = (ComboBoxItem)_localeComboBox.SelectedItem;
            var cultureCode = comboBoxItem.Name;
            await InitializeLocaleDataAsync(cultureCode);
            UpdateForm();
        }

        private void _launcherVersionLabel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _coreApi.OpenLauncherReleasePage();
        }
    }
}
