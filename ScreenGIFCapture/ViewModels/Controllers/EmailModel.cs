namespace ScreenGIFCapture.ViewModels.Controllers
{
    public class EmailModel : NotifyPropertyChanged
    {
        private string _body;
        private string _subject;
        private string _toAddress;

        public string Body
        {
            get => _body;
            set => Set(ref _body, value);
        }

        public string Subject
        {
            get => _subject;
            set => Set(ref _subject, value);
        }

        public string ToAddress
        {
            get => _toAddress;
            set => Set(ref _toAddress, value);
        }
    }
}