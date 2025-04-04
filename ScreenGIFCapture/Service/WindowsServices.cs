using ScreenGIFCapture.Screen;

namespace ScreenGIFCapture.Service
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;

    public class WindowsServices : IServices
    {
        public Rectangle DesktopRectangle => SystemInformation.VirtualScreen;

        public IEnumerable<IScreen> EnumerateScreens()
        {
            return ScreenWrapper.Enumerate();
        }

        public IBitmapImage Capture(Rectangle region)
        {
            return BitmapCapture.Capture(region);
        }
    }
}
