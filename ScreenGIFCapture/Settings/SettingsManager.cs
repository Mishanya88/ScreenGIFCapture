using System;
using System.IO;
using System.Xml;
using GifLibrary;
using Newtonsoft.Json;
using ScreenGIFCapture.ViewModels;

namespace ScreenGIFCapture.Settings
{
    public static class SettingsManager
    {
        private static readonly string SettingsFile = "settings.json";
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static void SaveSettings(MainViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            var settings = new SettingsModel
            {
                Fps = viewModel.Fps,
                SelectedCodec = viewModel.SelectedCodec.ToString()
            };

            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settings, SerializerSettings));
        }

        public static SettingsModel LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return CreateDefaultSettings();

            try
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonConvert.DeserializeObject<SettingsModel>(json);

                // Валидация и установка значений по умолчанию при необходимости
                if (settings.Fps <= 0) settings.Fps = 10;

                if (!Enum.TryParse(settings.SelectedCodec, out GifQuality _))
                    settings.SelectedCodec = GifQuality.Bit8.ToString();

                return settings;
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException)
            {
                // В случае ошибки возвращаем настройки по умолчанию
                return CreateDefaultSettings();
            }
        }

        private static SettingsModel CreateDefaultSettings()
        {
            return new SettingsModel
            {
                Fps = 10,
                SelectedCodec = GifQuality.Bit8.ToString()
            };
        }
    }
}