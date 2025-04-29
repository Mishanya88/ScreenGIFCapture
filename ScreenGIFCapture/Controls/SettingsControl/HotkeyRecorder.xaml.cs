namespace ScreenGIFCapture.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using GifCapture.Native;
    using ScreenGIFCapture.Settings;

    /// <summary>
    /// Логика взаимодействия для HotkeyRecorder.xaml
    /// </summary>
    public partial class HotkeyRecorder : Window
    {
        private readonly TextBox targetTextBox;

        private uint[] keys = new uint[2]; // [0] — Key, [1] — Modifiers
        private List<string> keyNames = new List<string>();
        private bool clearHotkey = false;
        private bool hotkeyHasBeenRecorded = false;
        private readonly List<Key> pressedModifiers = new List<Key>();

        public uint[] CapturedKeys => keys;

        public HotkeyRecorder(TextBox target)
        {
            InitializeComponent();
            targetTextBox = target;
            Loaded += HotkeyRecorder_Loaded;
            targetTextBox.TextChanged += TargetTextBox_TextChanged;
            MuteShortcut.Text = targetTextBox.Text;
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
                keys[0] = (uint)KeyInterop.VirtualKeyFromKey(key);
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
                        keys[1] |= (uint)User32.Modifiers.Control;
                        if (!keyNames.Contains("Ctrl")) keyNames.Add("Ctrl");
                        break;

                    case Key.LeftShift:
                    case Key.RightShift:
                        keys[1] |= (uint)User32.Modifiers.Shift;
                        if (!keyNames.Contains("Shift")) keyNames.Add("Shift");
                        break;

                    case Key.LeftAlt:
                    case Key.RightAlt:
                        keys[1] |= (uint)User32.Modifiers.Alt;
                        if (!keyNames.Contains("Alt")) keyNames.Add("Alt");
                        break;

                    case Key.LWin:
                    case Key.RWin:
                        keys[1] |= (uint)User32.Modifiers.Win;
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
    }
}
