using ScreenGIFCapture.Service;

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
    using System.Windows.Interop;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RecordBar _recordBar;
        private OverlayWindow _overlayWindow;
        private EmailWindow _emailWindow;
        private HotkeyManager _hotkeyManager;
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

            Loaded += (sender, args) =>
            {
                var helper = new WindowInteropHelper(this);
                var hwnd = helper.Handle;

                if (hwnd == IntPtr.Zero)
                {
                    MessageBox.Show("Ошибка: Нулевой дескриптор окна.");
                    return;
                }

                _hotkeyManager = new HotkeyManager(hwnd, ViewModel);
                _hotkeyManager.RegionHotkeyPressed += ToggleRegionRecording;
                _hotkeyManager.FullScreenHotkeyPressed += ToggleFullScreenRecording;
                _hotkeyManager.WindowHotkeyPressed += ToggleWindowRecording;
                _hotkeyManager.PauseHotkeyPressed += () =>
                {
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
                };
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

        public void UpdateHotKeys()
        {
            _hotkeyManager?.UpdateHotkeys();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _hotkeyManager?.Dispose();
            base.OnClosed(e);

            #if RELEASE

            if (!_toExit)
            {
                e.Cancel = true;
                Hide();
            }
            #endif
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
                await StartRecording(rectangle, mainViewModel);
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
                await StartRecording(captureArea.Value, mainViewModel); ;
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

                mainViewModel.Recoding = true;
                await StartRecording(target.Rectangle, mainViewModel);
            }
        }

        private async Task StartRecording(Rectangle area, MainViewModel viewModel)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string file = Path.Combine(viewModel.FilePath, $"{date}.gif");

            _recordBar?.Close();
            _recordBar = new RecordBar(viewModel, area);
            _recordBar.Show();

            await Task.Run(() => ToRecord(area, 1000 / viewModel.Fps, file, viewModel));

            if (viewModel.IsEmailEnabled)
            {
                _emailWindow?.Close();
                EmailWindow.ShowIfValid(viewModel, file);
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
                            gifCreator.AddFrame(img, Math.Max(10, currentDelay), quality: mainViewModel.SelectedCodec);
                            img.Dispose();
                            Dispatcher.Invoke(() => { mainViewModel.ElapsedSeconds =
                                (int)sw.Elapsed.TotalSeconds; });
                        }
                    }
                }
                sw.Stop();
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("imagePath", gifPath);
                NotificationsService.Notice("Запись с экрана была сохранена",
                    $"Местоположение：{gifPath}", gifPath, args);

            });

            Task.WaitAll(task1);
            _isStop = false;
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
            SettingsWindow settingsWindow = new SettingsWindow(ViewModel, _hotkeyManager);
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

        private void ToggleRegionRecording()
        {
            if (ViewModel.Recoding)
            {
                StopScreenClick(this, null);
            }
            else
            {
                RecordRegionClick(this, null);
            }
        }

        private void ToggleFullScreenRecording()
        {
            if (ViewModel.Recoding)
            {
                StopScreenClick(this, null);
            }
            else
            {
                RecordScreenClick(this, null);
            }
        }

        private void ToggleWindowRecording()
        {
            if (ViewModel.Recoding)
            {
                StopScreenClick(this, null);
            }
            else
            {
                RecordWindowClick(this, null);
            }
        }
    }
}