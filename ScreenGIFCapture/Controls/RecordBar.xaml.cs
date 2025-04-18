namespace ScreenGIFCapture.Controls
{
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.ViewModels;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    /// <summary>
    /// Логика взаимодействия для RecordBar.xaml
    /// </summary>
    public partial class RecordBar : System.Windows.Window
    {
        private readonly int _width = 200;
        private readonly int _height = 30;
        private bool _isPaused = false;

        public RecordBar(MainViewModel mainViewModel, Rectangle rectangle)
        {
            this.DataContext = mainViewModel;
            InitializeComponent();
            Rectangle screen = SystemInformation.VirtualScreen;
            int left = (int)((rectangle.X + rectangle.Width / 2) / Dpi.X - _width / 2);
            int top = (int)((rectangle.Y + rectangle.Height) / Dpi.Y + 10);
            if (top > screen.Height / Dpi.Y - _height)
            {
                top = (int)(screen.Height / Dpi.Y - _height);
            }

            if (left > screen.Width / Dpi.X - _width)
            {
                left = (int)(screen.Width / 2 / Dpi.X - _width / 2);
            }

            this.Top = top;
            this.Left = left;
            this.Width = _width;
            this.Height = _height;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void StopButtonOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.StopScreenClick(null, null);
            this.Close();
        }

        private void PauseResumeButtonClick(object sender, RoutedEventArgs e)
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                PauseResumeButton.Content = "▶";
                PauseResumeButton.ToolTip = "Продолжить запись";
                MainWindow.Instance.PauseRecording();
            }
            else
            {
                PauseResumeButton.Content = "⏸";
                PauseResumeButton.ToolTip = "Пауза";
                MainWindow.Instance.ResumeRecording();
            }
        }
    }
}
