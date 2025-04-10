namespace GifCapture.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using GifCapture.Native.Enums;
    using GifCapture.Native.Structs;
    using Point = System.Drawing.Point;

    public static class User32
    {
        private const string DllName = "user32.dll";

        [DllImport(DllName)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport(DllName)]
        public static extern bool DrawIconEx(
            IntPtr hDC,
            int left,
            int top,
            IntPtr hIcon,
            int width,
            int height,
            int stepIfAniCur,
            IntPtr brushForFlickerFreeDraw,
            DrawIconExFlags flags);

        [DllImport(DllName)]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, GetWindowLongValue nIndex);

        [DllImport(DllName)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport(DllName)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(DllName)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(DllName)]
        public static extern bool EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport(DllName)]
        public static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsProc proc, IntPtr lParam);

        [DllImport(DllName)]
        public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString,
            int nMaxCount);

        [DllImport(DllName)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowEnum uCmd);

        [DllImport(DllName)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport(DllName)]
        public static extern bool GetCursorInfo(ref CursorInfo pci);

        [DllImport(DllName)]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo piconinfo);

        [DllImport(DllName)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport(DllName)]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport(DllName)]
        public static extern bool FillRect(IntPtr hDC, ref RECT rect, IntPtr brush);

        [DllImport(DllName)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y,
            int cx, int cy, SetWindowPositionFlags wFlags);

        [DllImport(DllName)]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}