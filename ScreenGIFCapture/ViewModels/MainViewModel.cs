namespace ScreenGIFCapture.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private bool _recoding = false;

        public bool Recoding
        {
            get => _recoding;
            set => Set(ref _recoding, value);
        }
    }
}