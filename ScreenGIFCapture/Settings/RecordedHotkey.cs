namespace ScreenGIFCapture.Settings
{
    using static ScreenGIFCapture.Controls.HotkeyRecorder;
    using System.Collections.Generic;
    using System.Windows.Input;

    public class RecordedHotkey
    {
        public int Key { get; set; }
        public int Modifiers { get; set; }

        public override string ToString()
        {
            var parts = new List<string>();
            if ((Modifiers & (int)WindowsHotkeys.KeyModifier.Ctrl) != 0) parts.Add("Ctrl");
            if ((Modifiers & (int)WindowsHotkeys.KeyModifier.Shift) != 0) parts.Add("Shift");
            if ((Modifiers & (int)WindowsHotkeys.KeyModifier.Alt) != 0) parts.Add("Alt");
            if ((Modifiers & (int)WindowsHotkeys.KeyModifier.Win) != 0) parts.Add("Win");
            parts.Add(((Key)Key).ToString());
            return string.Join("+", parts);
        }
    }
}