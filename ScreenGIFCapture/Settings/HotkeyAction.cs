using System;

namespace ScreenGIFCapture.Settings
{
    public class HotkeyAction
    {
        public int Key { get; set; }
        public int Modifiers { get; set; }
        public Action Callback { get; set; }
    }
}