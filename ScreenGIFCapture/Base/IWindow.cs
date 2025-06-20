﻿namespace ScreenGIFCapture.Base
{
    using System;
    using System.Drawing;

    public interface IWindow
    {
        bool IsAlive { get; }

        bool IsVisible { get; }

        bool IsMaximized { get; }

        IntPtr Handle { get; }

        string Title { get; }

        Rectangle Rectangle { get; }
    }
}