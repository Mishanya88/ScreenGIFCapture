namespace ScreenGIFCapture.Screen
{
    using System.Drawing;
    using ScreenGIFCapture.Images;

    public static class BitmapCapture
    {
        public static IBitmapImage Capture(Rectangle rectangle)
        {
            return new BitmapImage(CreateBitmap(rectangle));
        }

        private static Bitmap CreateBitmap(Rectangle rectangle)
        {
            var bmp = new Bitmap(rectangle.Width, rectangle.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rectangle.Location, Point.Empty,
                    rectangle.Size, CopyPixelOperation.SourceCopy);

                g.Flush();
            }

            return bmp;
        }
    }
}