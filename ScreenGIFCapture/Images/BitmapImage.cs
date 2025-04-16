namespace ScreenGIFCapture.Images
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class BitmapImage : IBitmapImage
    {
        public Image Image { get;}

        public int Width => Image.Width;

        public int Height => Image.Height;

        public BitmapImage(Image image)
        {
            Image = image;
        }

        public void Save(string fileName, ImageFormat format)
        {
            Image.Save(fileName, format);
        }

        public void Save(Stream stream, ImageFormat format)
        {
            Image.Save(stream, format);
        }

        public void Dispose()
        {
            Image.Dispose();
        }
    }
}