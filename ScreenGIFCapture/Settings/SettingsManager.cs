namespace ScreenGIFCapture.Settings
{
    using System;
    using System.IO;
    using System.Windows.Input;
    using GifCapture.Native;
    using GifLibrary;
    using Newtonsoft.Json;
    using ScreenGIFCapture.ViewModels;

    public static class SettingsManager
    {
        private const string SettingsFile = "settings.json";

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void UpdateSetting<T>(string propertyName, T value)
        {
            var settings = LoadSettings();

            if (propertyName == nameof(SettingsModel.EncryptedPassword) && value is string password)
            {
                value = (T)(object)PasswordProtector.Encrypt(password);
            }

            var property = typeof(SettingsModel).GetProperty(propertyName);
            if (property?.CanWrite == true)
            {
                property.SetValue(settings, value);
            }

            SaveToFile(settings);
        }

        public static void SaveSettings(MainViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var settingsLoad = LoadSettings();

            var settings = new SettingsModel
            {
                Fps = viewModel.Fps,
                SelectedCodec = viewModel.SelectedCodec.ToString(),
                FilePath = viewModel.FilePath,
                RegionHotkey = viewModel.RegionHotkey,
                FullScreenHotkey = viewModel.FullScreenHotkey,
                PauseHotkey = viewModel.PauseHotkey,
                RecordWindowHotkey = viewModel.RecordWindowHotkey,
                EncryptedPassword = PasswordProtector.Encrypt(viewModel.SenderPassword),
                SenderEmail = viewModel.SenderEmail,
                BodyEmail = viewModel.BodyEmail,
                Subject = viewModel.Subject,
                SmtpPort = viewModel.SmtpPort,
                SmtpServer = viewModel.SmtpServer,
                ToAddress = settingsLoad.ToAddress
            };

            SaveToFile(settings);
        }

        public static SettingsModel LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return CreateDefaultSettings();

            try
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonConvert.DeserializeObject<SettingsModel>(json) ??
                               CreateDefaultSettings();

                settings.RegionHotkey ??= CreateHotkey(User32.Modifiers.Control, Key.S);
                settings.FullScreenHotkey ??= CreateHotkey(User32.Modifiers.Control, Key.F);
                settings.PauseHotkey ??= CreateHotkey(User32.Modifiers.Alt, Key.S);
                settings.RecordWindowHotkey ??= CreateHotkey(User32.Modifiers.Control, Key.W);

                if (settings.Fps <= 0)
                {
                    settings.Fps = 10;
                }

                if (settings.SmtpPort <= 0)
                {
                    settings.SmtpPort = 587;
                }

                if (!Enum.TryParse(settings.SelectedCodec, out GifQuality _))
                {
                    settings.SelectedCodec = GifQuality.Bit8.ToString();
                }

                settings.FilePath = GetDefaultSavePath();

                return settings;
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException)
            {
                Console.Error.WriteLine($"[SettingsManager] Ошибка чтения настроек: {ex.Message}");
                return CreateDefaultSettings();
            }
        }

        public static string GetDefaultSavePath()
        {
            var picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var savePath = Path.Combine(picturesFolder, "ScreenGIF");

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            return savePath;
        }

        private static SettingsModel CreateDefaultSettings() => new SettingsModel
        {
            Fps = 10,
            SelectedCodec = GifQuality.Bit8.ToString(),
            FilePath = GetDefaultSavePath(),
            RegionHotkey = CreateHotkey(User32.Modifiers.Control, Key.R),
            FullScreenHotkey = CreateHotkey(User32.Modifiers.Control, Key.F),
            RecordWindowHotkey = CreateHotkey(User32.Modifiers.Control, Key.W),
            PauseHotkey = CreateHotkey(User32.Modifiers.Alt, Key.S),
            EncryptedPassword = string.Empty,
            BodyEmail = "Gif прилагается.",
            Subject = "Отправка электронной почты с GifRecord",
            SmtpServer = "smtp.gmail.com",
            SenderEmail = string.Empty,
            SmtpPort = 587,
            ToAddress = string.Empty
        };

        private static RecordedHotkey CreateHotkey(User32.Modifiers modifier,
            Key key) => new RecordedHotkey
        {
            Modifiers = (uint)modifier,
            Key = (uint)KeyInterop.VirtualKeyFromKey(key)
        };

        private static void SaveToFile(SettingsModel settings)
        {
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settings,
                SerializerSettings));
        }
    }
}
