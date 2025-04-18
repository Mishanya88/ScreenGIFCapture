using ScreenGIFCapture.Screen;

namespace ScreenGIFCapture.Service
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using GifCapture.Native;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Images;

    public class WindowsServices : IServices
    {
        public Rectangle DesktopRectangle => SystemInformation.VirtualScreen;

        public Point CursorPosition
        {
            get
            {
                var p = new Point();
                User32.GetCursorPos(ref p);
                return p;
            }
        }

        public IEnumerable<IScreen> EnumerateScreens()
        {
            return ScreenWrapper.Enumerate();
        }

        public IBitmapImage Capture(Rectangle region)
        {
            return BitmapCapture.Capture(region);
        }

        public IEnumerable<IWindow> EnumerateAllWindows()
        {
            return Window
                .Enumerate()
                .Where(w => w.IsVisible)
                .SelectMany(GetAllChildren);
        }

        public IEnumerable<IWindow> EnumerateWindows()
        {
            return Window.EnumerateVisible();
        }

        IEnumerable<Window> GetAllChildren(Window window)
        {
            var children = window
                .EnumerateChildren()
                .Where(w => w.IsVisible);

            foreach (var child in children)
            {
                foreach (var grandchild in GetAllChildren(child))
                {
                    yield return grandchild;
                }
            }

            yield return window;
        }
    }
}
