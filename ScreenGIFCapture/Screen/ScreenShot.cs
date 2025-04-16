namespace ScreenGIFCapture.Screen
{
    using System.Drawing;
    using ScreenGIFCapture.Images;
    using ScreenGIFCapture.Service;

    public static class ScreenShot
    {
        public static IBitmapImage Capture()
        {
            var platformServices = ServiceProvider.IServicesPlatform;
            return CaptureImage(platformServices.DesktopRectangle);
        }

        public static IBitmapImage CaptureImage(Rectangle rectangle)
        {
            var platform = ServiceProvider.IServicesPlatform;
            return platform.Capture(rectangle);
        }
    }
}