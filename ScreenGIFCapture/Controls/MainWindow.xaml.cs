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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel;
        public bool _isStop = false;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
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
                await Task.Run(() => ToRecord(rectangle, file));
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
                            Bitmap img = provider.Capture();
                            gifCreator.AddFrame(img, quality: GifQuality.Bit8);
                            img.Dispose();
                        }
                    }
                }
            });

            Task.WaitAll(task1);
            _isStop = false;
        }

        private void StopScreenClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.Recoding = false;
            }
            _isStop = true;
        }

        private void RecordRegionClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                IScreen screen = ScreenWindow.GetScreen();

                Rectangle? rectangle = RegionWindow.PickRegion();


            }
        }
    }
}