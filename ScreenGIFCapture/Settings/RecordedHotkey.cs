namespace ScreenGIFCapture.Settings
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using GifCapture.Native;

    public class RecordedHotkey
    {
        public uint Key { get; set; } 
        public uint Modifiers { get; set; }

        public override string ToString()
        {
            var parts = new List<string>();

            if ((Modifiers & (uint)User32.Modifiers.Control) != 0) parts.Add("Ctrl");
            if ((Modifiers & (uint)User32.Modifiers.Shift) != 0) parts.Add("Shift");
            if ((Modifiers & (uint)User32.Modifiers.Alt) != 0) parts.Add("Alt");
            if ((Modifiers & (uint)User32.Modifiers.Win) != 0) parts.Add("Win");

            parts.Add(KeyInterop.KeyFromVirtualKey((int)Key).ToString());

            return string.Join("+", parts);
        }
    }
}