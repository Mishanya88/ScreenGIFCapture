namespace ScreenGIFCapture.Service
{
    using ScreenGIFCapture.Base;
    using System.Collections.Generic;
    using System.Drawing;


    public interface IServices
    {
        IEnumerable<IScreen> EnumerateScreens();
        Rectangle DesktopRectangle { get; }
        //IBitmapImage Capture(Rectangle region, bool includeCursor = false);
    }
}
