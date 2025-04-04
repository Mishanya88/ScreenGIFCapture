using System.Drawing.Imaging;

namespace ScreenGIFCapture.Images
{
    using System;

    public interface IBitmapImage : IDisposable
    {
        int Width { get; }

        int Height { get; }

        void Save(string fileName, ImageFormat format);

    }
}
