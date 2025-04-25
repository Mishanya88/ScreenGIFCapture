namespace ScreenGIFCapture.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using ScreenGIFCapture.Settings;

    /// <summary>
    /// Логика взаимодействия для HotkeyRecorder.xaml
    /// </summary>
    public partial class HotkeyRecorder : Window
    {
        private readonly TextBox targetTextBox;

        private int[] keys = new int[2]; // [0] — Key, [1] — Modifiers
        private List<string> keyNames = new List<string>();
        private bool clearHotkey = false;
        private bool hotkeyHasBeenRecorded = false;
        private readonly List<Key> pressedModifiers = new List<Key>();

        public int[] CapturedKeys => keys;

        public HotkeyRecorder(TextBox target)
        {
            InitializeComponent();
            targetTextBox = target;
            Loaded += HotkeyRecorder_Loaded;
            targetTextBox.TextChanged += TargetTextBox_TextChanged;
            MuteShortcut.Text = targetTextBox.Text;
        }

        public static HashSet<Key> ConvertToKeySet(RecordedHotkey hotkey)
        {
            var keys = new HashSet<Key>();

            if ((hotkey.Modifiers & (int)HotkeyRecorder.WindowsHotkeys.KeyModifier.Ctrl) != 0)
                keys.Add(Key.LeftCtrl);
            if ((hotkey.Modifiers & (int)HotkeyRecorder.WindowsHotkeys.KeyModifier.Shift) != 0)
                keys.Add(Key.LeftShift);
            if ((hotkey.Modifiers & (int)HotkeyRecorder.WindowsHotkeys.KeyModifier.Alt) != 0)
                keys.Add(Key.LeftAlt);
            if ((hotkey.Modifiers & (int)HotkeyRecorder.WindowsHotkeys.KeyModifier.Win) != 0)
                keys.Add(Key.LWin);

            keys.Add((Key)hotkey.Key);
            return keys;
        }

        private void HotkeyRecorder_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;

            if (IsModifierKey(key) && !pressedModifiers.Contains(key))
                pressedModifiers.Add(key);

            switch (key)
            {
                case Key.Escape:
                    CloseRecorder();
                    break;

                case Key.Return:
                    if (hotkeyHasBeenRecorded)
                    {
                        targetTextBox.Text = string.Join("+", keyNames);
                    }
                    CloseRecorder();
                    break;

                default:
                    Record(e);
                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;

            if (pressedModifiers.Contains(key))
                pressedModifiers.Remove(key);

            e.Handled = true;
            clearHotkey = !KeysDown().Any();

            if (!hotkeyHasBeenRecorded)
            {
                RecordAndDisplayModKeys();
            }
        }

        private async void Record(KeyEventArgs e)
        {
            Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;

            e.Handled = true;

            if (hotkeyHasBeenRecorded && !clearHotkey)
                return;

            if (hotkeyHasBeenRecorded && clearHotkey)
            {
                Array.Clear(keys, 0, keys.Length);
                keyNames.Clear();
                hotkeyHasBeenRecorded = false;
            }

            RecordAndDisplayModKeys();

            if (!IsModifierKey(key))
            {
                keys[0] = (int)key;
                keyNames.Add(key.ToString());
                hotkeyHasBeenRecorded = true;

                string display = string.Join("+", keyNames);
                targetTextBox.Text = display;

                await Task.Delay(100);
                CloseRecorder();

                HotkeySaved?.Invoke(new RecordedHotkey
                {
                    Key = keys[0],
                    Modifiers = keys[1]
                });
            }
        }

        private void RecordAndDisplayModKeys()
        {
            keyNames.Clear();
            keys[1] = 0;

            foreach (Key modKey in pressedModifiers)
            {
                switch (modKey)
                {
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        keys[1] |= (int)WindowsHotkeys.KeyModifier.Ctrl;
                        if (!keyNames.Contains("Ctrl")) keyNames.Add("Ctrl");
                        break;

                    case Key.LeftShift:
                    case Key.RightShift:
                        keys[1] |= (int)WindowsHotkeys.KeyModifier.Shift;
                        if (!keyNames.Contains("Shift")) keyNames.Add("Shift");
                        break;

                    case Key.LeftAlt:
                    case Key.RightAlt:
                        keys[1] |= (int)WindowsHotkeys.KeyModifier.Alt;
                        if (!keyNames.Contains("Alt")) keyNames.Add("Alt");
                        break;

                    case Key.LWin:
                    case Key.RWin:
                        keys[1] |= (int)WindowsHotkeys.KeyModifier.Win;
                        if (!keyNames.Contains("Win")) keyNames.Add("Win");
                        break;
                }
            }

            string display = string.Join("+", keyNames);
            targetTextBox.Text = display;
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LWin || key == Key.RWin;
        }

        private static IEnumerable<Key> KeysDown()
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (key != Key.None && Keyboard.IsKeyDown(key))
                    yield return key;
            }
        }

        private void CloseRecorder()
        {
            if (Owner != null)
            {
                Owner.IsHitTestVisible = true;
                Owner.ResizeMode = ResizeMode.CanResize;
            }
            Close();
        }

        private void TargetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MuteShortcut.Text = targetTextBox.Text;
        }

        public event Action<RecordedHotkey> HotkeySaved;

        public static class WindowsHotkeys
        {
            [Flags]
            public enum KeyModifier
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                Win = 8
            }
        }
    }
}
