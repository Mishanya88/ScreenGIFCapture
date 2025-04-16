namespace ScreenGIFCapture.Service
{
    using System.Collections.Generic;
    using System.Drawing;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;

    public interface IServices
    {
        IEnumerable<IScreen> EnumerateScreens();

        IEnumerable<IWindow> EnumerateAllWindows();

        Rectangle DesktopRectangle { get; }

        IBitmapImage Capture(Rectangle region);
    }
}
