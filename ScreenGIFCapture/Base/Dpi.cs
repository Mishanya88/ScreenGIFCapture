namespace ScreenGIFCapture.Base
{
    using System.Windows.Interop;

    public class Dpi
    {
        static Dpi()
        {
            using (var src = new HwndSource(new HwndSourceParameters()))
            {
                if (src.CompositionTarget != null)
                {
                    var matrix = src.CompositionTarget.TransformToDevice;

                    X = (float)matrix.M11;
                    Y = (float)matrix.M22;
                }
            }
        }

        public static float X { get; }

        public static float Y { get; }
    }
}