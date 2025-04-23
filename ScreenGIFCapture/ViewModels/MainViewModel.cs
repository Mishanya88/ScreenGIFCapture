using GifLibrary;
using ScreenGIFCapture.Settings;

namespace ScreenGIFCapture.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private bool _recoding = false;
        private int _elapsedSeconds;
        private int _delayIndex;
        private int _fps = 10;
        private string _filePath = SettingsManager.GetDefaultSavePath();
        private GifQuality _selectedCodec = GifQuality.Bit8;

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
    }
}