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
    using System;

    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            RegionCaptureHotkeyTextBox.Text = viewModel.RegionHotkey.ToString();
            FullScreenCaptureHotkeyTextBox.Text = viewModel.FullScreenHotkey.ToString();
            TogglePauseHotkeyTextBox.Text = viewModel.PauseHotkey.ToString();
            this.DataContext = viewModel;
        }

        private void HotkeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var recorder = new HotkeyRecorder(textBox);
                recorder.Owner = this;

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

        public event Action<string, RecordedHotkey> HotkeyChanged;
    }
}
