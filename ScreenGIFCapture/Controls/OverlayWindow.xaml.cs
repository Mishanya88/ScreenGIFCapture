namespace ScreenGIFCapture.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Логика взаимодействия для OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        public OverlayWindow(Rect bounds)
        {
            InitializeComponent();

            Left = 0;
            Top = 0;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            Canvas.SetLeft(OverlayRect, bounds.Left - 2);
            Canvas.SetTop(OverlayRect, bounds.Top - 2);
            OverlayRect.Width = bounds.Width + 4;
            OverlayRect.Height = bounds.Height + 4;
        }
    }
}
