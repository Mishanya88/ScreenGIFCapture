namespace ScreenGIFCapture.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using ScreenGIFCapture.Base;

    public partial class PictureRegion : UserControl
    {
        public static readonly DependencyProperty OverlayVisibilityProperty =
            DependencyProperty.Register(nameof(OverlayVisibility), typeof(Visibility),
                typeof(PictureRegion), new PropertyMetadata(Visibility.Visible));
        
        private Rect? _region;

        public Rect? Region
        {
            get => _region;
            set
            {
                if (_region != value)
                {
                    _region = value;
                    OnRegionChanged(value);
                }
            }
        }

        public Visibility OverlayVisibility
        {
            get => (Visibility)GetValue(OverlayVisibilityProperty);
            set => SetValue(OverlayVisibilityProperty, value);
        }

        public PictureRegion()
        {
            InitializeComponent();
        }

        private void OnRegionChanged(Rect? region)
        {
            if (region == null)
            {
                Visibility = Visibility.Hidden;
                OverlayVisibility = Visibility.Visible;
                return;
            }

            double w = ActualWidth;
            double h = ActualHeight;

            var rect = region.Value;

            BorderTop.Margin = new Thickness();
            BorderTop.Width = w;
            BorderTop.Height = rect.Top.Clip(0, h);

            BorderBottom.Margin = new Thickness(0, rect.Bottom, 0, 0);
            BorderBottom.Width = w;
            BorderBottom.Height = (h - rect.Bottom).Clip(0, h);

            BorderLeft.Margin = new Thickness(0, rect.Top, 0, 0);
            BorderLeft.Width = rect.Left.Clip(0, w);
            BorderLeft.Height = rect.Height;

            BorderRight.Margin = new Thickness(rect.Right, rect.Top, 0, 0);
            BorderRight.Width = (w - rect.Right).Clip(0, w);
            BorderRight.Height = rect.Height;

            Visibility = Visibility.Visible;
            OverlayVisibility = Visibility.Hidden;
        }
    }
}
