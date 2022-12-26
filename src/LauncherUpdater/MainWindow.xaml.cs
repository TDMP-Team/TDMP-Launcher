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

        private float _percentage_changed;

        private string _status_text;

        public event PropertyChangedEventHandler PropertyChanged;

        public float PercentageChanged
        {
            get { return _percentage_changed; }
            set
            {
                _percentage_changed = value;
                OnPropertyChanged("PercentageChanged");
            }
        }

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
            System.Timers.Timer t = new();
            t.Interval = 100; // In milliseconds
            t.AutoReset = true; // Stops it from repeating
            t.Elapsed += new ElapsedEventHandler(TimerElapsed);
            t.Start();
        }

        void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            PercentageChanged = _coreApi.GetPercentageDone();
            StatusText = _coreApi.GetCurrentTask();
        }

        private void UpdateForm()
        {
            _updaterMainWindow.Title = "TDMP Launcher Update";

            _updaterVersionLabel.Content = $"Updater v{_coreApi.GetLauncherVersion()}";
        }
    }
}
