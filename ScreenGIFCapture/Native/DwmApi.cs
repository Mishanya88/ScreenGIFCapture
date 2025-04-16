namespace GifCapture.Native
{
    using System.Runtime.InteropServices;
    using System;
    using GifCapture.Native.Structs;

    public static class DwmApi
    {
        const string DllName = "dwmapi.dll";

        [DllImport(DllName)]
        public static extern int DwmGetWindowAttribute(IntPtr Window, int Attribute,
            out bool Value, int Size);

        [DllImport(DllName)]
        public static extern int DwmGetWindowAttribute(IntPtr Window, int Attribute,
            ref RECT Value, int Size);
    }
}