namespace ScreenGIFCapture.Controls
{
    using System;
    using System.IO;
    using System.Drawing.Imaging;
    using System.Windows;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;
    using ScreenGIFCapture.Screen;
    using System.Drawing;
    using System.Threading.Tasks;
    using ScreenGIFCapture.Gif;
    using ScreenGIFCapture.ViewModels;
    using GifLibrary;
    using Window = System.Windows.Window;
    using System.Threading;
    using System.Diagnostics;

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
        private OverlayWindow _overlayWindow;

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
                _overlayWindow?.Close();
                _overlayWindow = null;
                mainViewModel.ElapsedSeconds = 0;
            }
            _isStop = true;
        }

        private async Task DelayAsync(MainViewModel viewModel)
        {
            int delaySeconds;

            switch (viewModel.DelayIndex)
            {
                case 1:
                    delaySeconds = 2;
                    break;
                case 2:
                    delaySeconds = 4;
                    break;
                case 3:
                    delaySeconds = 6;
                    break;
                case 4:
                    delaySeconds = 8;
                    break;
                default:
                    delaySeconds = 0;
                    break;
            }

            viewModel.CountdownSeconds = delaySeconds;

            while (viewModel.CountdownSeconds > 0)
            {
                await Task.Delay(1000);
                viewModel.CountdownSeconds--;
            }
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
                await DelayAsync(mainViewModel);

                IScreen screen = ScreenWindow.GetScreen();
                Rectangle rectangle = screen.Rectangle;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, rectangle);
                _recordBar.Show();
                await Task.Run(() => ToRecord(rectangle, 1000 / mainViewModel.Fps, file, mainViewModel));
            }

        }

        private async void RecordRegionClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                Rectangle? captureArea = RegionWindow.PickRegion();
                if (captureArea == null) 
                {
                    return;
                }

                _overlayWindow = new OverlayWindow(
                    new Rect(captureArea.Value.Left, captureArea.Value.Top,
                        captureArea.Value.Width, captureArea.Value.Height));
                _overlayWindow.Show();

                await DelayAsync(mainViewModel);

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, captureArea.Value);
                _recordBar.Show();
                await Task.Run(() => ToRecord(captureArea.Value, 1000 / mainViewModel.Fps, file, mainViewModel));

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

                await DelayAsync(mainViewModel);

                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, target.Rectangle);
                _recordBar.Show();

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(desktop, $"{date}.gif");
                await Task.Run(() => ToRecord(target.Rectangle, 1000 / mainViewModel.Fps, file, mainViewModel));
            }
        }

        private void ToRecord(Rectangle rectangle, int delay, string gifPath, MainViewModel mainViewModel)
        {
            Task task1 = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var gifCreator = GifLibrary.AnimatedGif.Create(gifPath, delay))
                {
                    using (var provider = new GifRegionProvider(rectangle))
                    {
                        TimeSpan prev = sw.Elapsed;

                        while (_isStop != true)
                        {
                            var currentDelay = delay;
                            var curr = sw.Elapsed;
                            int usedt = (int)(curr - prev).TotalMilliseconds;
                            if (usedt < currentDelay)
                            {
                                int a = currentDelay - usedt;
                                Sleep(a);
                            }
                            else
                            {
                                currentDelay = usedt;
                            }

                            if (_isPaused)
                            {
                                sw.Stop();
                                Thread.Sleep(100);
                                continue;
                            }
                            else
                            {
                                sw.Start();
                            }

                            prev = curr;

                            Bitmap img = provider.Capture();
                            gifCreator.AddFrame(img, currentDelay, quality: GifQuality.Bit8);
                            img.Dispose();
                            Dispatcher.Invoke(() => { mainViewModel.ElapsedSeconds =
                                (int)sw.Elapsed.TotalSeconds; });
                        }
                    }
                }
                sw.Stop();
            });

            Task.WaitAll(task1);
            _isStop = false;
        }

        private void Sleep(int ms)
        {
            var sw = Stopwatch.StartNew();
            var sleepMs = ms - 16;
            if (sleepMs > 0)
            {
                Thread.Sleep(sleepMs);
            }

            while (sw.ElapsedMilliseconds < ms)
            {
                Thread.SpinWait(1);
            }
        }
    }
}