namespace ScreenGIFCapture.Controls
{
    using System;
    using System.IO;
    using System.Windows;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Screen;
    using System.Drawing;
    using System.Threading.Tasks;
    using ScreenGIFCapture.Gif;
    using ScreenGIFCapture.ViewModels;
    using Window = System.Windows.Window;
    using System.Threading;
    using System.Diagnostics;
    using GifCapture.Native;
    using ScreenGIFCapture.Utils;
    using ScreenGIFCapture.Settings;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RecordBar _recordBar;
        private OverlayWindow _overlayWindow;
        private EmailWindow _emailWindow;
        private bool _isStop = false;
        private bool _isPaused = false;
        private bool _toExit = false;

        public MainViewModel ViewModel;
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            Instance = this;
            DataContext = ViewModel;

            this.Loaded += (sender, args) =>
            {
                var helper = new System.Windows.Interop.WindowInteropHelper(this);
                var hwnd = helper.Handle;

                if (hwnd == IntPtr.Zero)
                {
                    MessageBox.Show("Ошибка: Нулевой дескриптор окна.");
                    return;
                }

                var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
                source.AddHook(HwndHook);

                UpdateHotKeys(hwnd);
            };
        }

        public void PauseRecording() => _isPaused = true;

        public void ResumeRecording() => _isPaused = false;

        public void StopScreenClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.Recoding = false;
                _recordBar?.Close();
                _recordBar = null;
                _overlayWindow?.Close();
                _overlayWindow = null;
                mainViewModel.ElapsedSeconds = 0;
            }
            _isStop = true;
        }

        public void UpdateHotKeys(IntPtr hwnd)
        {
            User32.UnregisterHotKey(hwnd, 1);
            User32.UnregisterHotKey(hwnd, 2);
            User32.UnregisterHotKey(hwnd, 3);
            User32.UnregisterHotKey(hwnd, 4);

            RegisterHotKeyForAction(hwnd, ViewModel.RegionHotkey, 1);
            RegisterHotKeyForAction(hwnd, ViewModel.FullScreenHotkey, 2);
            RegisterHotKeyForAction(hwnd, ViewModel.PauseHotkey, 3);
            RegisterHotKeyForAction(hwnd, ViewModel.RecordWindowHotkey, 4);
        }

        public void UpdateHotKeys()
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            User32.UnregisterHotKey(hwnd, 1);
            User32.UnregisterHotKey(hwnd, 2);
            User32.UnregisterHotKey(hwnd, 3);
            User32.UnregisterHotKey(hwnd, 4);

            RegisterHotKeyForAction(hwnd, ViewModel.RegionHotkey, 1);
            RegisterHotKeyForAction(hwnd, ViewModel.FullScreenHotkey, 2);
            RegisterHotKeyForAction(hwnd, ViewModel.PauseHotkey, 3);
            RegisterHotKeyForAction(hwnd, ViewModel.RecordWindowHotkey, 4);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            var helper = new System.Windows.Interop.WindowInteropHelper(this);
            User32.UnregisterHotKey(helper.Handle, 1);
            base.OnClosed(e);

            //if (!_toExit)
            //{
            //    e.Cancel = true;
            //    Hide();
            //}
        }

        private async void RecordScreenClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.Recoding = true;

                int delaySeconds = GetDelaySeconds(mainViewModel.DelayIndex);
                if (delaySeconds > 0)
                {
                    var countdownWindow = new CountdownWindow(delaySeconds);
                    countdownWindow.ShowDialog();
                }

                IScreen screen = ScreenWindow.GetScreen();
                Rectangle rectangle = screen.Rectangle;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(mainViewModel.FilePath, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, rectangle);
                _recordBar.Show();
                await Task.Run(() => ToRecord(rectangle, 1000 / mainViewModel.Fps, file, mainViewModel));

                if (mainViewModel.IsEmailEnabled)
                {
                    _emailWindow?.Close();
                    _emailWindow?.Close();
                    EmailWindow.ShowIfValid(mainViewModel, file);
                }
            }
        }

        private async void RecordRegionClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                Rectangle? captureArea = RegionWindow.PickRegion();
                if (captureArea == null) 
                {
                    return;
                }

                _overlayWindow = new OverlayWindow(
                    new Rect(captureArea.Value.Left, captureArea.Value.Top,
                        captureArea.Value.Width, captureArea.Value.Height));
                _overlayWindow.Show();
                int delaySeconds = GetDelaySeconds(mainViewModel.DelayIndex);

                if (delaySeconds > 0)
                {
                    var countdownWindow = new CountdownWindow(delaySeconds);
                    countdownWindow.ShowDialog();
                }

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(mainViewModel.FilePath, $"{date}.gif");
                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, captureArea.Value);
                _recordBar.Show();
                await Task.Run(() => ToRecord(captureArea.Value, 1000 / mainViewModel.Fps, file, mainViewModel));

                if (mainViewModel.IsEmailEnabled)
                {
                    _emailWindow?.Close();
                    _emailWindow?.Close();
                    EmailWindow.ShowIfValid(mainViewModel, file);
                }
            }
        }

        private async void RecordWindowClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                IWindow target = VideoSourcePickerWindow.PickWindow();

                if (target == null)
                {
                    return;
                }

                int delaySeconds = GetDelaySeconds(mainViewModel.DelayIndex);
                if (delaySeconds > 0)
                {
                    var countdownWindow = new CountdownWindow(delaySeconds);
                    countdownWindow.ShowDialog();
                }

                _recordBar?.Close();
                _recordBar = new RecordBar(mainViewModel, target.Rectangle);
                _recordBar.Show();

                mainViewModel.Recoding = true;
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string file = Path.Combine(mainViewModel.FilePath, $"{date}.gif");
                await Task.Run(() => ToRecord(target.Rectangle, 1000 / mainViewModel.Fps, file, mainViewModel));

                if (mainViewModel.IsEmailEnabled)
                {
                    _emailWindow?.Close();
                    EmailWindow.ShowIfValid(mainViewModel, file);
                }
            }
        }

        private void ToRecord(Rectangle rectangle, int delay, string gifPath, MainViewModel mainViewModel)
        {
            Task task1 = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var gifCreator = GifLibrary.AnimatedGif.Create(gifPath, delay))
                {
                    using (var provider = new GifRegionProvider(rectangle))
                    {
                        TimeSpan prev = sw.Elapsed;

                        while (_isStop != true)
                        {
                            var currentDelay = delay;
                            var curr = sw.Elapsed;
                            int usedt = (int)(curr - prev).TotalMilliseconds;
                            if (usedt < currentDelay)
                            {
                                int a = currentDelay - usedt;
                                Sleep(a);
                            }
                            else
                            {
                                currentDelay = usedt;
                            }

                            if (_isPaused)
                            {
                                sw.Stop();
                                Thread.Sleep(100);
                                continue;
                            }
                            else
                            {
                                sw.Start();
                            }

                            prev = curr;

                            Bitmap img = provider.Capture();
                            gifCreator.AddFrame(img, currentDelay, quality: mainViewModel.SelectedCodec);
                            img.Dispose();
                            Dispatcher.Invoke(() => { mainViewModel.ElapsedSeconds =
                                (int)sw.Elapsed.TotalSeconds; });
                        }
                    }
                }
                sw.Stop();
            });

            Task.WaitAll(task1);
            _isStop = false;
        }

        private void RegisterHotKeyForAction(IntPtr hwnd, RecordedHotkey hotkey, int id)
        {
            if (hotkey != null)
            {
                User32.RegisterHotKey(hwnd, id, hotkey.Modifiers, hotkey.Key);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                switch (id)
                {
                    case 1:
                        RecordRegionClick(this, null);
                        break;
                    case 2:
                        RecordScreenClick(this, null);
                        break;
                    case 3:
                        if (ViewModel.Recoding)
                        {
                            if (_isPaused)
                            {
                                ResumeRecording();
                            }
                            else
                            {
                                PauseRecording();
                            }
                        }
                        break;
                    case 4:
                        RecordWindowClick(this, null);
                        break;
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        private int GetDelaySeconds(int delayIndex)
        {
            switch (delayIndex)
            {
                case 1:
                    return 2;
                case 2:
                    return 4;
                case 3:
                    return 6;
                case 4:
                    return 8;
                default:
                    return 0;
            }
        }

        private void Sleep(int ms)
        {
            var sw = Stopwatch.StartNew();
            var sleepMs = ms - 16;
            if (sleepMs > 0)
            {
                Thread.Sleep(sleepMs);
            }

            while (sw.ElapsedMilliseconds < ms)
            {
                Thread.SpinWait(1);
            }
        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(ViewModel);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void TaskbarIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                this.ShowAndFocus();
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            _toExit = true;
            Close();
        }
    }
}