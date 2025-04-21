namespace ScreenGIFCapture.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private bool _recoding = false;
        private int _elapsedSeconds;
        private int _delayIndex;
        private int _countdownSeconds;

        public readonly int Fps = 10;

        public bool Recoding
        {
            get => _recoding;
            set => Set(ref _recoding, value);
        }

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            set => Set(ref _elapsedSeconds, value);
        }

        public int DelayIndex
        {
            get => _delayIndex;
            set => Set(ref _delayIndex, value);
        }
    }
}