namespace ScreenGIFCapture.Images
{
    using System.Drawing;
    using System.Drawing.Imaging;

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

        public void Dispose()
        {
            Image.Dispose();
        }
    }
}