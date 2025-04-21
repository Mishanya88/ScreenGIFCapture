using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenGIFCapture.Controls
{
    public partial class CountdownWindow : Window, INotifyPropertyChanged
    {
        private int _countdownSeconds;

        public int CountdownSeconds
        {
            get => _countdownSeconds;
            set
            {
                _countdownSeconds = value;
                OnPropertyChanged(nameof(CountdownSeconds));
            }
        }

        public CountdownWindow(int countdownSeconds)
        {
            InitializeComponent();
            DataContext = this;
            CountdownSeconds = countdownSeconds;
            StartCountdown();
        }

        private async void StartCountdown()
        {
            while (CountdownSeconds > 0)
            {
                await Task.Delay(1000);
                CountdownSeconds--;
            }

            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}