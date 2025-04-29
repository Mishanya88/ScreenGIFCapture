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
        private string _smtpServer = "smtp.gmail.com";
        private int _smtpPort = 587;
        private string _subject;
        private string _senderEmail;
        private string _senderPassword;
        private string _bodyEmail;
        private bool _isEmailEnabled;
        private bool _rememberLastRecipient;

        public bool RememberLastRecipient
        {
            get => _rememberLastRecipient;
            set => Set(ref _rememberLastRecipient, value);
        }

        public bool IsEmailEnabled
        {
            get => _isEmailEnabled;
            set => Set(ref _isEmailEnabled, value);
        }

        public string SmtpServer
        {
            get => _smtpServer;
            set => Set(ref _smtpServer, value);
        }

        public int SmtpPort
        {
            get => _smtpPort;
            set => Set(ref _smtpPort, value);
        }

        public string Subject
        {
            get => _subject;
            set => Set(ref _subject, value);
        }

        public string SenderEmail
        {
            get => _senderEmail;
            set => Set(ref _senderEmail, value);
        }

        public string SenderPassword
        {
            get => _senderPassword;
            set => Set(ref _senderPassword, value);
        }

        public string BodyEmail
        {
            get => _bodyEmail;
            set => Set(ref _bodyEmail, value);
        }

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
            SenderPassword = PasswordProtector.Decrypt(settings.EncryptedPassword);
            BodyEmail = settings.BodyEmail;
            Subject = settings.Subject;
            SenderEmail = settings.SenderEmail;
            SmtpPort = settings.SmtpPort;
            SmtpServer = settings.SmtpServer;
        }
    }
}