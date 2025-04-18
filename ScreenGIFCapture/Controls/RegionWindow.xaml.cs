namespace ScreenGIFCapture.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;
    using ScreenGIFCapture.Screen;
    using ScreenGIFCapture.Service;
    using BitmapImage = System.Windows.Media.Imaging.BitmapImage;
    using Point = System.Windows.Point;
    using Window = System.Windows.Window;


    /// <summary>
    /// Логика взаимодействия для RegionWindow.xaml
    /// </summary>
    public partial class RegionWindow : Window
    {
        private readonly IWindow[] _windows;
        private readonly IServices _platformServices;
        private IWindow _selectedWindow;
        private Point? _start, _end;
        private bool _isDragging;
        private Predicate<IWindow> Predicate { get; set; }

        public RegionWindow()
        {
            InitializeComponent();
            _platformServices = ServiceProvider.IServicesPlatform;
            _windows = _platformServices.EnumerateAllWindows().ToArray();

            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            UpdateBackground();
        }

        public static Rectangle? PickRegion()
        {
            var window = new RegionWindow();
            window.ShowDialog();
            
            return window.GetRegionScaled();
        }

        private void UpdateBackground()
        {
            using (IBitmapImage b = ScreenShot.Capture())
            {
                using (Stream stream = new MemoryStream())
                {
                    b.Save(stream, ImageFormat.Bmp);
                    stream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    BgImg.Source = bitmapImage;
                }
            }
        }

        private Rectangle? GetRegionScaled()
        {
            var rect = GetRegion();

            if (rect == null)
            {
                return null;
            }

            Rect r = rect.Value;

            return new Rectangle(
                (int)((this.Left + r.X) * Dpi.X),
                (int)((this.Top + r.Y) * Dpi.Y),
                (int)(r.Width * Dpi.X),
                (int)(r.Height * Dpi.Y));
        }

        private Rect? GetRegion()
        {
            if (_start == null || _end == null)
            {
                return null;
            }

            Point end = _end.Value;
            Point start = _start.Value;

            if (end.X < start.X)
            {
                (start.X, end.X) = (end.X, start.X);
            }

            if (end.Y < start.Y)
            {
                (start.Y, end.Y) = (end.Y, start.Y);
            }

            double width = end.X - start.X;
            double height = end.Y - start.Y;

            if (width < 0.01 || height < 0.01)
            {
                return null;
            }

            return new Rect(start.X, start.Y, width, height);
        }

        private void CursorMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            _end = e.GetPosition(RootGrid);
            Rect? r = GetRegion();
            if (r == null)
            {
                Unhighlight();
                return;
            }

            HighlightRegion(r.Value);
        }

        private void Unhighlight()
        {
            PuncturedRegion.Region = null;
        }

        private void HighlightRegion(Rect region)
        {
            PuncturedRegion.Region = region;
        }

        private void CursorMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _start = e.GetPosition(RootGrid);
            _end = null;
        }

        private Rect? GetSelectedWindowRectangle()
        {
            if (_selectedWindow == null)
            {
                return null;
            }

            var rect = _selectedWindow.Rectangle;
            return new Rect(
                -Left + rect.X / Dpi.X,
                -Top + rect.Y / Dpi.Y,
                rect.Width / Dpi.X,
                rect.Height / Dpi.Y);
        }

        private void CursorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
                return;

            var current = e.GetPosition(RootGrid);

            if (current != _start)
            {
                _end = e.GetPosition(RootGrid);
            }
            else if (GetSelectedWindowRectangle() is Rect rect)
            {
                _start = rect.Location;
                _end = new Point(rect.Right, rect.Bottom);
            }

            Close();
        }

        private void CloseClick(object sender, ExecutedRoutedEventArgs e)
        {
            _start = _end = null;
            Close();
        }   
    }
}
