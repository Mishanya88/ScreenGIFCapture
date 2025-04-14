namespace ScreenGIFCapture.Gif
{
    using System;
    using System.Drawing;
    using GifCapture.Native;

    public class GdiTargetDeviceContext : ITargetDeviceContext
    {
        private readonly IntPtr _hdcDest, _hBitmap;

        public GdiTargetDeviceContext(IntPtr srcDc, int width, int height)
        {
            _hdcDest = Gdi32.CreateCompatibleDC(srcDc);
            _hBitmap = Gdi32.CreateCompatibleBitmap(srcDc, width, height);
            Gdi32.SelectObject(_hdcDest, _hBitmap);
        }

        public Bitmap GetBitmap()
        {
            return Image.FromHbitmap(_hBitmap);
        }

        public IntPtr GetDc()
        {
            return _hdcDest;
        }

        public void Dispose()
        {
            Gdi32.DeleteDC(_hdcDest);
            Gdi32.DeleteObject(_hBitmap);
        }
    }
}