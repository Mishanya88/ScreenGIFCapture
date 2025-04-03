namespace ScreenGIFCapture.Service
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using ScreenGIFCapture.Base;


    internal class WindowsServices : IServices
    {
        public Rectangle DesktopRectangle => SystemInformation.VirtualScreen;

        public IEnumerable<IScreen> EnumerateScreens()
        {
            throw new System.NotImplementedException();
        }
    }
}
