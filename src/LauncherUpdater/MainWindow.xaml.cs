using System.ComponentModel;
using System.Timers;
using System.Windows;
using LauncherUpdater.Core;

namespace LauncherUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly CoreApi _coreApi = new CoreApi();

        public event PropertyChangedEventHandler PropertyChanged;

        private float _percentage_changed;
        public float PercentageChanged
        {
            get { return _percentage_changed; }
            set
            {
                _percentage_changed = value;
                OnPropertyChanged("PercentageChanged");
            }
        }

        private string _status_text;
        public string StatusText
        {
            get { return _status_text; }
            set
            {
                _status_text = value;
                OnPropertyChanged("StatusText");
            }
        }

        protected virtual void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _coreApi.InitializeAsync();
            UpdateForm();

            Timer t = new();
            t.Interval = 100; // In milliseconds
            t.AutoReset = true; // Stops it from repeating
            t.Elapsed += new ElapsedEventHandler((sender, e) =>
            {
                PercentageChanged = _coreApi.GetPercentageDone();
                StatusText = _coreApi.GetCurrentTask();
            });
            t.Start();

            await _coreApi.SetUpAndLaunchLauncherAsync();
        }

        private void UpdateForm()
        {
            _updaterVersionLabel.Content = $"TDMP Launcher Updater v{_coreApi.GetLauncherVersion()}";
        }
    }
}
