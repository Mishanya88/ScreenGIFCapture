namespace ScreenGIFCapture.Settings
{
    using GifCapture.Native;
    using ScreenGIFCapture.ViewModels;
    using System;
    using System.Windows.Interop;

    public class HotkeyManager : IDisposable
    {
        private readonly IntPtr _hwnd;
        private readonly MainViewModel _viewModel;
        private readonly HwndSource _source;
        
        private bool _isPaused = false;

        public HotkeyManager(IntPtr hwnd, MainViewModel viewModel)
        {
            _hwnd = hwnd;
            _viewModel = viewModel;

            _source = HwndSource.FromHwnd(_hwnd);
            _source.AddHook(WndProc);

            RegisterHotkeys();
        }

        public void Pause()
        {
            if (_isPaused)
            {
                return;
            }
            _isPaused = true;

            User32.UnregisterHotKey(_hwnd, 1);
            User32.UnregisterHotKey(_hwnd, 2);
            User32.UnregisterHotKey(_hwnd, 3);
            User32.UnregisterHotKey(_hwnd, 4);
        }

        public void Resume()
        {
            if (!_isPaused)
            {
                return;
            }
            _isPaused = false;

            RegisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            User32.UnregisterHotKey(_hwnd, 1);
            User32.UnregisterHotKey(_hwnd, 2);
            User32.UnregisterHotKey(_hwnd, 3);
            User32.UnregisterHotKey(_hwnd, 4);

            RegisterHotKey(_viewModel.RegionHotkey, 1);
            RegisterHotKey(_viewModel.FullScreenHotkey, 2);
            RegisterHotKey(_viewModel.PauseHotkey, 3);
            RegisterHotKey(_viewModel.RecordWindowHotkey, 4);
        }

        public void UpdateHotkeys() => RegisterHotkeys();

        private void RegisterHotKey(RecordedHotkey hotkey, int id)
        {
            if (hotkey != null)
            {
                User32.RegisterHotKey(_hwnd, id, hotkey.Modifiers, hotkey.Key);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                handled = true;

                switch (id)
                {
                    case 1: RegionHotkeyPressed?.Invoke(); break;
                    case 2: FullScreenHotkeyPressed?.Invoke(); break;
                    case 3: PauseHotkeyPressed?.Invoke(); break;
                    case 4: WindowHotkeyPressed?.Invoke(); break;
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            User32.UnregisterHotKey(_hwnd, 1);
            User32.UnregisterHotKey(_hwnd, 2);
            User32.UnregisterHotKey(_hwnd, 3);
            User32.UnregisterHotKey(_hwnd, 4);
        }

        public event Action RegionHotkeyPressed;
        public event Action FullScreenHotkeyPressed;
        public event Action PauseHotkeyPressed;
        public event Action WindowHotkeyPressed;
    }
}
