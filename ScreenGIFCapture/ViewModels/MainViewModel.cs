namespace ScreenGIFCapture.ViewModels
{
    using GifLibrary;
    using ScreenGIFCapture.Controls;
    using ScreenGIFCapture.Settings;

    public class MainViewModel : NotifyPropertyChanged
    {
        private bool _recoding = false;
        private int _elapsedSeconds;
        private int _delayIndex;
        private int _fps = 10;
        private string _filePath = SettingsManager.GetDefaultSavePath();
        private GifQuality _selectedCodec = GifQuality.Bit8;
        private RecordedHotkey _regionHotkey;
        private RecordedHotkey _fullScreenHotkey;
        private RecordedHotkey _pauseHotkey;
        private RecordedHotkey _recordWindowHotkey;

        public RecordedHotkey RegionHotkey
        {
            get => _regionHotkey;
            set
            {
                if (Set(ref _regionHotkey, value))
                {
                    MainWindow.Instance?.UpdateHotKeys();
                }
            }
        }

        public RecordedHotkey FullScreenHotkey
        {
            get => _fullScreenHotkey;
            set
            {
                if (Set(ref _fullScreenHotkey, value))
                {
                    MainWindow.Instance?.UpdateHotKeys();
                }
            }
        }

        public RecordedHotkey PauseHotkey
        {
            get => _pauseHotkey;
            set
            {
                if (Set(ref _pauseHotkey, value))
                {
                    MainWindow.Instance?.UpdateHotKeys();
                }
            }
        }

        public RecordedHotkey RecordWindowHotkey
        {
            get => _recordWindowHotkey;
            set
            {
                if (Set(ref _recordWindowHotkey, value))
                {
                    MainWindow.Instance?.UpdateHotKeys();
                }
            }
        }

        public int Fps
        {
            get => _fps;
            set => Set(ref _fps, value);
        }

        public int DelayIndex
        {
            get => _delayIndex;
            set => Set(ref _delayIndex, value);
        }

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

        public GifQuality SelectedCodec
        {
            get => _selectedCodec;
            set => Set(ref _selectedCodec, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        public MainViewModel()
        {
            var settings = SettingsManager.LoadSettings();
            Fps = settings.Fps;
            SelectedCodec = settings.GetCodec();
            FilePath = settings.FilePath;
            RegionHotkey = settings.RegionHotkey;
            PauseHotkey = settings.PauseHotkey;
            FullScreenHotkey = settings.FullScreenHotkey;
            RecordWindowHotkey = settings.RecordWindowHotkey;
        }
    }
}