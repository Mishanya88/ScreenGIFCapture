using System.Drawing;
using System.Threading.Tasks;
using ScreenGIFCapture.Gif;
using ScreenGIFCapture.ViewModels;

namespace ScreenGIFCapture.Controls
{
    using System;
    using System.IO;
    using System.Drawing.Imaging;
    using System.Windows;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;
    using ScreenGIFCapture.Screen;
    using GifLibrary;
    using Window = System.Windows.Window;
    using System.Net.NetworkInformation;
    using System.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel;
        public bool _isStop = false;
        public bool _isPaused = false;
        public static MainWindow Instance { get; private set; }
        private RecordBar _recordBar;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            Instance = this;
            DataContext = ViewModel;
        }

        public void PauseRecording() => _isPaused = true;
        public void ResumeRecording() => _isPaused = false;

        public void StopScreenClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.Recoding = false;
                _recordBar?.Close();
                _recordBar = null;
            }
            _isStop = true;
        }

        private void ScreenButtonClick(object sender, RoutedEventArgs e)
        {
            IScreen screen = ScreenWindow.GetScreen();

            if (screen != null)
            {
                IBitmapImage img = ScreenShot.CaptureImage(screen.Rectangle);
                SaveScreenShot(img);
            }
        }

        private void SaveScreenShot(IBitmapImage img)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string file = Path.Combine(desktop, $"{date}.png");
            img.Save(file, ImageFormat.Png);
        }

        private async void RecordScreenClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.Recoding = true;
                IScreen screen = ScreenWindow.GetScreen();
                Rectangle rectangle = screen.Rectangle;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, rectangle);
                _recordBar.Show();
                await Task.Run(() => ToRecord(rectangle, file));
            }

        }

        private async void RecordRegionClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                Rectangle? rectangle = RegionWindow.PickRegion();
                if (rectangle == null) 
                {
                    return;
                }

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, rectangle.Value);
                _recordBar.Show();
                await Task.Run(() => ToRecord(rectangle.Value, file));

            }
        }

        private async void RecordWindowClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                IWindow target = VideoSourcePickerWindow.PickWindow();

                if (target == null)
                {
                    return;
                }

                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, target.Rectangle);
                _recordBar.Show();

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                await Task.Run(() => ToRecord(target.Rectangle, file));
            }
        }

        private void ToRecord(Rectangle rectangle, string gifPath)
        {

            Task task1 = Task.Run(() =>
            {
                using (var gifCreator = GifLibrary.AnimatedGif.Create(gifPath))
                {
                    using (var provider = new GifRegionProvider(rectangle))
                    {
                        while (_isStop != true)
                        {
                            if (_isPaused)
                            {
                                Thread.Sleep(100);
                                continue;
                            }

                            Bitmap img = provider.Capture();
                            gifCreator.AddFrame(img, 100, quality: GifQuality.Bit8);
                            img.Dispose();
                        }
                    }
                }
            });

            Task.WaitAll(task1);
            _isStop = false;
        }

    }
}