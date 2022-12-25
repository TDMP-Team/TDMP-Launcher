using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using TeardownMultiplayerLauncher.Core;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Repositories;

namespace TeardownMultiplayerLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CoreApi _coreApi = new CoreApi();
        private LanguageTranslations translations;

        private Dictionary<string, string> languageCodeMap = new Dictionary<string, string>
            {
                { "Russian", "ru" },
                { "English", "en" }
            };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task LoadLanguage()
        {
            translations = await LanguageRepository.GetLanguage(_coreApi.GetLanguage());
        }

        private void InitializeLauncherVersionLabel()
        {
            _launcherVersionLabel.Content = string.Format(translations.LauncherVersionText, _coreApi.GetLauncherVersion());
        }

        private void UpdateForm()
        {
            var isGameVersionSupported = _coreApi.HasSupportedTeardownVersion();

            _versionSupportLabel.Content = isGameVersionSupported switch
            {
                true => translations.TeardownValidText,
                false => string.Format(translations.TeardownInvalidText, _coreApi.GetSupportedTeardownVersion()),
                null => translations.SelectTeardownText,
            };
            _versionSupportLabel.Background = isGameVersionSupported switch
            {
                true => new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x88, 0x08)),
                false => new SolidColorBrush(Color.FromArgb(0xFF, 0x88, 0x00, 0x25)),
                null => new SolidColorBrush(Color.FromArgb(0xFF, 0xA9, 0xA3, 0x00)),
            };

            _launcherMainWindow.Title = translations.LauncherTitle;

            _injectionTitleLabel.Content = translations.InjectionTitleText;
            _injectionDescriptionLabel.Content = translations.InjectionDescriptionText;

            _teardownPathLabel.Content = translations.TeardownPathText;

            _joinDiscordLabel.Content = translations.JoinDiscordText;

            _teardownFolderBrowseButton.Content = translations.BrowseText;

            _teardownFolderTextBox.Content = _coreApi.GetTeardownExePath();

            _playButton.Content = translations.PlayButtonText;
            _playButton.IsEnabled = isGameVersionSupported == true;

            _teardownMultiplayerVersionLabel.Content = $"TDMP v{_coreApi.GetInstalledTeardownMultiplayerVersion()}";

            _injectionDelaySlider.Value = _coreApi.GetInjectionDelay().TotalSeconds;
            _injectionDelayLabel.Content = string.Format(translations.SecondsText, _coreApi.GetInjectionDelay().TotalSeconds);

            switch (_coreApi.GetLanguage())
            {
                case ("ru"):
                    _englishLanguageSelection.IsSelected = false;
                    _russianlanguageSelection.IsSelected = true;
                    break;
                case ("en"):
                default:
                    _englishLanguageSelection.IsSelected = true;
                    _russianlanguageSelection.IsSelected = false;
                    break;
            }
        }

        private async void _teardownFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = translations.SelectDialogText;
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
                    SetBusyStatusText(translations.UpdatingTDMPText);
                    await _coreApi.SetUpLatestTeardownMultiplayerReleaseAsync();
                    UpdateForm();
                }

                SetBusyStatusText(translations.GameRunningText);
                await _coreApi.LaunchTeardownMultiplayer();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format(translations.LaunchErrorText, ex), translations.LauncherTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _busyStatusGrid.Visibility = Visibility.Hidden;
        }

        private void SetBusyStatusText(string text)
        {
            _busyStatusLabel.Content = text;
        }

        private async void LanguageChanged()
        {
            if (!languageCodeMap.ContainsValue(_coreApi.GetLanguage()))
            {
                _coreApi.SetLanguageAsync("en");
            }
            await LoadLanguage();
            InitializeLauncherVersionLabel();
            UpdateForm();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _coreApi.InitializeAsync();
            await LoadLanguage();
            InitializeLauncherVersionLabel();
            UpdateForm();
        }

        private void _discordGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _coreApi.OpenDiscordServer();
        }

        private async void _injectionDelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            await _coreApi.SetInjectionDelayAsync(TimeSpan.FromSeconds(e.NewValue));
            UpdateForm();
        }

        private void _languageSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            ComboBoxItem item = (ComboBoxItem)e.AddedItems[0]; // ComboBoxes don't *require* ComboBoxItems as children, but this is a safe cast.
            if (item == null || item.Content == null || item.Content.ToString() == "") return;
            if (languageCodeMap[item.Content.ToString()] == _coreApi.GetLanguage()) return;
            //System.Windows.Controls.ComboBox box = (System.Windows.Controls.ComboBox)sender;
            _coreApi.SetLanguageAsync(languageCodeMap[item.Content.ToString()]);
            LanguageChanged();
        }
    }
}
