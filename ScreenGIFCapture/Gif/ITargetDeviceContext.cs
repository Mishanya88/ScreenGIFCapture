namespace ScreenGIFCapture.Gif
{
    using System;
    using System.Drawing;

    public interface ITargetDeviceContext : IDisposable
    {
        IntPtr GetDc();

        Bitmap GetBitmap();
    }
}