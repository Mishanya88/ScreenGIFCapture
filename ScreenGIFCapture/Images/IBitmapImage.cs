using System.Drawing.Imaging;

namespace ScreenGIFCapture.Images
{
    using System;
    using System.IO;

    public interface IBitmapImage : IDisposable
    {
        int Width { get; }

        int Height { get; }

        void Save(string fileName, ImageFormat format);

        void Save(Stream stream, ImageFormat format);
    }
}
