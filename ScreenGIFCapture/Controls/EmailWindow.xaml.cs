using ScreenGIFCapture.Settings;
using System;
using System.Windows;
using ScreenGIFCapture.ViewModels;
using ScreenGIFCapture.ViewModels.Controllers;

namespace ScreenGIFCapture.Controls
{
    /// <summary>
    /// Логика взаимодействия для EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        private readonly MainViewModel _model;
        private readonly string _filePath;
        public EmailModel ViewEmail;

        public EmailWindow(MainViewModel model, string filePath)
        {
            InitializeComponent();
            ViewEmail = new EmailModel();
            _model = model;
            _filePath = filePath;
            ViewEmail.Subject = _model.Subject;
            ViewEmail.Body = _model.BodyEmail;

            if (_model.RememberLastRecipient)
            {
                var settings = SettingsManager.LoadSettings();
                ViewEmail.ToAddress = settings.ToAddress;
            }

            DataContext = ViewEmail;
        }

        public static void ShowIfValid(MainViewModel model, string filePath)
        {
            var missingFields = "";

            if (string.IsNullOrWhiteSpace(model.SmtpServer))
            {
                missingFields += "SMTP-сервер\n";
            }
            if (string.IsNullOrWhiteSpace(model.SenderEmail))
            {
                missingFields += "Адрес отправителя\n";
            }
            if (string.IsNullOrWhiteSpace(model.SenderPassword))
            {
                missingFields += "Пароль\n";
            }

            if (!string.IsNullOrEmpty(missingFields))
            {
                MessageBox.Show("Отправка по SMTP невозможна, следующие поля не заполнены:\n" +
                                missingFields, "Неверные настройки",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new EmailWindow(model, filePath);
            window.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendButton.IsEnabled = false;
            SendingProgressBar.Visibility = Visibility.Visible;

            try
            {
                var mailer = new Mailer(_model.SmtpServer, _model.SmtpPort, _model.SenderEmail,
                    _model.SenderPassword);

                await mailer.SendEmail(ViewEmail.ToAddress, ViewEmail.Subject, ViewEmail.Body, _filePath);

                MessageBox.Show("Письмо отправлено успешно!", "Успех", MessageBoxButton.OK,
                    MessageBoxImage.Information);

                if (_model.RememberLastRecipient)
                {
                    SettingsManager.UpdateSetting(nameof(ViewEmail.ToAddress), ViewEmail.ToAddress);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось отправить письмо: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            finally
            {
                SendingProgressBar.Visibility = Visibility.Collapsed;
                SendButton.IsEnabled = true;
            }
        }
    }
}
