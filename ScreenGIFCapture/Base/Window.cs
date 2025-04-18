namespace ScreenGIFCapture.Base
{
    using GifCapture.Native;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using GifCapture.Native.Structs;
    using System.Collections.Generic;
    using System.Linq;
    using GifCapture.Native.Enums;

    public class Window : IWindow
    {
        public bool IsAlive => User32.IsWindow(Handle);

        public bool IsVisible => User32.IsWindowVisible(Handle);

        public bool IsMaximized => User32.IsZoomed(Handle);

        public IntPtr Handle { get; }

        public static Window DesktopWindow { get; } = new Window(User32.GetDesktopWindow());

        public string Title {
            get 
            {
                var title = new StringBuilder(User32.GetWindowTextLength(Handle) + 1);
                User32.GetWindowText(Handle, title, title.Capacity);
                return title.ToString();
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                var r = new RECT();
                const int extendedFrameBounds = 9;
                if (DwmApi.DwmGetWindowAttribute(Handle, extendedFrameBounds, ref r,
                        Marshal.SizeOf<RECT>()) != 0)
                {
                    if (!User32.GetWindowRect(Handle, out r))
                    {
                        return Rectangle.Empty;
                    }
                }

                return r.ToRectangle();
            }
        }

        public Window(IntPtr handle)
        {
            if (!User32.IsWindow(handle))
            {
                throw new ArgumentException("Not a Window.", nameof(handle));
            }

            this.Handle = handle;
        }

        public IEnumerable<Window> EnumerateChildren()
        {
            var list = new List<Window>();
            User32.EnumChildWindows(Handle, (Handle, Param) =>
            {
                var wh = new Window(Handle);
                list.Add(wh);
                return true;
            }, IntPtr.Zero);

            return list;
        }

        public static IEnumerable<Window> Enumerate()
        {
            var list = new List<Window>();
            User32.EnumWindows((handle, param) =>
            {
                var wh = new Window(handle);
                list.Add(wh);
                return true;
            }, IntPtr.Zero);

            return list;
        }

        public static IEnumerable<Window> EnumerateVisible()
        {
            foreach (var window in Enumerate().Where(w => w.IsVisible && !string.IsNullOrWhiteSpace(w.Title)))
            {
                var hWnd = window.Handle;

                if (!User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.AppWindow))
                {
                    if (User32.GetWindow(hWnd, GetWindowEnum.Owner) != IntPtr.Zero)
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.ExStyle).HasFlag(WindowStyles.ToolWindow))
                        continue;

                    if (User32.GetWindowLong(hWnd, GetWindowLongValue.Style).HasFlag(WindowStyles.Child))
                        continue;
                }

                const int dwmCloaked = 14;

                // Exclude suspended Windows apps
                DwmApi.DwmGetWindowAttribute(hWnd, dwmCloaked, out var cloaked, Marshal.SizeOf<bool>());

                if (cloaked)
                    continue;

                yield return window;
            }
        }
    }
}