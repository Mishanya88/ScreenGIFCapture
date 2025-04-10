namespace GifCapture.Native
{
    using ScreenGIFCapture.Native.Enums;
    using System;
    using System.Runtime.InteropServices;

    static class Gdi32
    {
        private const string DllName = "gdi32.dll";

        [DllImport(DllName)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport(DllName)]
        public static extern bool BitBlt(IntPtr hObject, int xDest, int yDest, int width, int height,
            IntPtr objectSource, int xSrc, int ySrc, int op);

        [DllImport(DllName)]
        public static extern bool StretchBlt(IntPtr hObject, int xDest, int yDest, int wDest,
            int hDest, IntPtr objectSource, int xSrc, int ySrc, int wSrc,
            int hSrc, int op);

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int width, int height);

        [DllImport(DllName)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport(DllName)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport(DllName)]
        public static extern uint SetDCBrushColor(IntPtr hDC, uint color); //color=0x00bbggrr

        [DllImport(DllName)]
        public static extern uint SetDCPenColor(IntPtr hDC, uint color);

        [DllImport(DllName)]
        public static extern bool Ellipse(IntPtr hDC, int left, int top, int right, int bottom);

        [DllImport(DllName)]
        public static extern IntPtr GetStockObject(StockObjects fnObject);
    }
}