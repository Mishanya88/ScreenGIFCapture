namespace ScreenGIFCapture
{
    using System;
    using System.IO;
    using System.Drawing.Imaging;
    using System.Windows;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;
    using ScreenGIFCapture.Screen;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}