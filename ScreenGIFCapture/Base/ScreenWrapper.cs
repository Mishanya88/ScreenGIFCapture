namespace ScreenGIFCapture.Base
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ScreenWrapper : IScreen
    {
        private readonly System.Windows.Forms.Screen _screen;

        public ScreenWrapper(System.Windows.Forms.Screen screen)
        {
            _screen = screen;
        }

        public string Name => _screen.DeviceName;

        public Rectangle Rectangle => _screen.Bounds;

        public static IEnumerable<IScreen> Enumerate()
        {
            return System.Windows.Forms.Screen.AllScreens.Select(device =>
                new ScreenWrapper(device));
        }

    }
}
