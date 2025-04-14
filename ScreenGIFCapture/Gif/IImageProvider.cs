namespace ScreenGIFCapture.Gif
{
    using System;
    using System.Drawing;

    internal interface IImageProvider : IDisposable
    {
        Bitmap Capture();

        int Height { get; }

        int Width { get; }
    }
}
