namespace GifCapture.Native.Structs
{
    using System.Drawing;

    public static class NativeExtensions
    {
        public static Rectangle ToRectangle(this RECT r) => Rectangle.FromLTRB(r.Left, r.Top,
            r.Right, r.Bottom);
    }
}