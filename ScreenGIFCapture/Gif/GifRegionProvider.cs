namespace ScreenGIFCapture.Gif
{
    using System;
    using System.Drawing;
    using GifCapture.Native;

    public class GifRegionProvider : IImageProvider
    {
        private readonly Rectangle _rectangle;
        private readonly IntPtr _hdcSrc;
        private readonly ITargetDeviceContext _dcTarget;

        public int Width { get;}
        public int Height { get;}

        public GifRegionProvider(Rectangle rectangle)
        {
            _rectangle = rectangle;
            Height = rectangle.Height;
            Width = rectangle.Width;

            _hdcSrc = User32.GetDC(IntPtr.Zero);
            _dcTarget = new GdiTargetDeviceContext(_hdcSrc, _rectangle.Width, _rectangle.Height);
        }

        private void OnCapture()
        {
            Rectangle rect = _rectangle;
            IntPtr hdcDest = _dcTarget.GetDc();

            Gdi32.StretchBlt(hdcDest, 0, 0, Width, Height,
                _hdcSrc, rect.X, rect.Y, Width, Height,
                (int)CopyPixelOperation.SourceCopy);
        }

        public Bitmap Capture()
        {
            OnCapture();
            Bitmap img = _dcTarget.GetBitmap();
            return img;
        }

        public void Dispose()
        {
            _dcTarget.Dispose();
        }
    }
}
