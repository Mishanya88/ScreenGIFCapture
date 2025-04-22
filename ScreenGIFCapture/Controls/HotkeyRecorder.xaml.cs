using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenGIFCapture.Controls
{
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

        public int[] CapturedKeys => keys;

        public HotkeyRecorder(TextBox target)
        {
            InitializeComponent();
            targetTextBox = target;
            Loaded += HotkeyRecorder_Loaded;
        }

        private void HotkeyRecorder_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
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
            e.Handled = true;
            clearHotkey = !KeysDown().Any();

            if (!hotkeyHasBeenRecorded)
            {
                RecordAndDisplayModKeys();
            }
        }

        private void Record(KeyEventArgs e)
        {
            Key key = (e.Key == Key.System) ? e.SystemKey : e.Key;

            if (key == Key.LWin || key == Key.RWin)
                return;

            e.Handled = true;

            if (hotkeyHasBeenRecorded)
            {
                if (clearHotkey)
                {
                    Array.Clear(keys, 0, keys.Length);
                    keyNames.Clear();
                    hotkeyHasBeenRecorded = false;
                }
            }
            else
            {
                if (!IsModifierKey(key))
                {
                    keys[0] = (int)key;
                    keyNames.Add(key.ToString());
                    hotkeyHasBeenRecorded = true;
                    targetTextBox.Text = string.Join("+", keyNames);

                    // Завершаем запись после ввода основной клавиши
                    CloseRecorder();
                }
                else
                {
                    RecordAndDisplayModKeys();
                }
            }
        }

        private void RecordAndDisplayModKeys()
        {
            keyNames.Clear();
            keys[1] = 0;

            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                keys[1] |= (int)WindowsHotkeys.KeyModifier.Ctrl;
                keyNames.Add("Ctrl");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                keys[1] |= (int)WindowsHotkeys.KeyModifier.Shift;
                keyNames.Add("Shift");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
            {
                keys[1] |= (int)WindowsHotkeys.KeyModifier.Alt;
                keyNames.Add("Alt");
            }

            targetTextBox.Text = string.Join("+", keyNames);
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LeftShift || key == Key.RightShift;
        }

        private static IEnumerable<Key> KeysDown()
        {
            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (key != Key.None && Keyboard.IsKeyDown(key) &&
                    key != Key.LWin && key != Key.RWin)
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
