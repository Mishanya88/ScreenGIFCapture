namespace ScreenGIFCapture.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using ScreenGIFCapture.Base;

    public partial class PictureRegion : UserControl
    {
        
        //public static readonly DependencyProperty RegionProperty = DependencyProperty.Register(
        //    nameof(Region),
        //    typeof(Rect?),
        //    typeof(PictureRegion),
        //    new PropertyMetadata(RegionChanged));

        public static readonly DependencyProperty OverlayVisibilityProperty =
            DependencyProperty.Register(nameof(OverlayVisibility), typeof(Visibility),
                typeof(PictureRegion), new PropertyMetadata(Visibility.Visible));

        //public Rect? Region
        //{
        //    get => (Rect?)GetValue(RegionProperty);
        //    set => SetValue(RegionProperty, value);
        //}

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

        private static void RegionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is PictureRegion r)
            {
                switch (e.NewValue)
                {
                    case null:
                        r.Visibility = Visibility.Hidden;
                        break;

                    case Rect region:
                        var w = r.ActualWidth;
                        var h = r.ActualHeight;

                        r.BorderTop.Margin = new Thickness();
                        r.BorderTop.Width = w;
                        r.BorderTop.Height = region.Top.Clip(0, h);

                        r.BorderBottom.Margin = new Thickness(0, region.Bottom, 0, 0);
                        r.BorderBottom.Width = w;
                        r.BorderBottom.Height = (h - region.Bottom).Clip(0, h);

                        r.BorderLeft.Margin = new Thickness(0, region.Top, 0, 0);
                        r.BorderLeft.Width = region.Left.Clip(0, w);
                        r.BorderLeft.Height = region.Height;

                        r.BorderRight.Margin = new Thickness(region.Right, region.Top, 0, 0);
                        r.BorderRight.Width = (w - region.Right).Clip(0, w);
                        r.BorderRight.Height = region.Height;

                        r.Visibility = Visibility.Visible;
                        r.OverlayVisibility = Visibility.Hidden;
                        break;
                }
            }
        }
    }
}
