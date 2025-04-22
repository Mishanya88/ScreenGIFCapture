using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;
using ScreenGIFCapture.Settings;
using ScreenGIFCapture.ViewModels;
using ComboBox = System.Windows.Controls.ComboBox;

namespace ScreenGIFCapture.Controls
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();

            var settings = SettingsManager.LoadSettings();
            viewModel.Fps = settings.Fps;
            viewModel.SelectedCodec = settings.GetCodec();

            this.DataContext = viewModel;
        }

        private void HotkeyTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var recorder = new HotkeyRecorder(textBox);
                recorder.Owner = this;
                this.IsHitTestVisible = false;
                this.ResizeMode = ResizeMode.NoResize;
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
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainViewModel)this.DataContext;

            SettingsManager.SaveSettings(viewModel);

            MessageBox.Show("Settings saved!");
            Close();
        }

        private void SettingsWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var viewModel = (MainViewModel)this.DataContext;
            SettingsManager.SaveSettings(viewModel);
        }
    }
}
