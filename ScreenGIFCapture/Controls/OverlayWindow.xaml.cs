using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GifCapture.Native;

namespace ScreenGIFCapture.Controls
{
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
