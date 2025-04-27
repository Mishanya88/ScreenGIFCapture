namespace ScreenGIFCapture.Utils
{
    using System.Windows;

    public static class WpfExtensions
    {
        public static void ShowAndFocus(this Window w)
        {
            if (w.IsVisible && w.WindowState == WindowState.Minimized)
            {
                w.WindowState = WindowState.Normal;
            }

            w.Show();

            w.Activate();
        }
    }
}