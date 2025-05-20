namespace ScreenGIFCapture.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Forms;
    using System.IO;
    using MessageBox = System.Windows.Forms.MessageBox;
    using TextBox = System.Windows.Controls.TextBox;
    using Window = System.Windows.Window;
    using ScreenGIFCapture.Settings;
    using ScreenGIFCapture.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private HotkeyManager _hotkeyManager;

        public SettingsWindow(MainViewModel viewModel, HotkeyManager hotkeyManager)
        {
            InitializeComponent();

            RegionCaptureHotkeyTextBox.Text = viewModel.RegionHotkey.ToString();
            FullScreenCaptureHotkeyTextBox.Text = viewModel.FullScreenHotkey.ToString();
            TogglePauseHotkeyTextBox.Text = viewModel.PauseHotkey.ToString();
            WindowCaptureHotkeyTextBox.Text = viewModel.RecordWindowHotkey.ToString();
            PasswordTextBox.Password = viewModel.SenderPassword;
            this.DataContext = viewModel;
            _hotkeyManager = hotkeyManager;
        }

        private void HotkeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _hotkeyManager.Pause();
                var recorder = new HotkeyRecorder(textBox)
                {
                    Owner = this
                };

                recorder.HotkeySaved += hotkey =>
                {
                    textBox.Text = hotkey.ToString();

                    if (textBox == RegionCaptureHotkeyTextBox)
                    {
                        MainWindow.Instance.ViewModel.RegionHotkey = hotkey;
                    }
                    else if (textBox == FullScreenCaptureHotkeyTextBox)
                    {
                        MainWindow.Instance.ViewModel.FullScreenHotkey = hotkey;
                    }
                    else if (textBox == TogglePauseHotkeyTextBox)
                    {
                        MainWindow.Instance.ViewModel.PauseHotkey = hotkey;
                    }
                    else if (textBox == WindowCaptureHotkeyTextBox)
                    {
                        MainWindow.Instance.ViewModel.RecordWindowHotkey = hotkey;
                    }
                };

                recorder.Closed += (_, __) =>
                {
                    _hotkeyManager.Resume();
                };

                recorder.ShowDialog();
            }
        }

        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Выберите папку для сохранения";

            var result = folderDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SaveFolderTextBox.Text = folderDialog.SelectedPath;
                MainWindow.Instance.ViewModel.FilePath = folderDialog.SelectedPath;
            }
        }

        private void SettingsWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var viewModel = (MainViewModel)this.DataContext;
            SettingsManager.SaveSettings(viewModel);
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            var path = SaveFolderTextBox.Text;

            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                MessageBox.Show("Папка не найдена. Возможно, она была удалена.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SenderPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SenderPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
