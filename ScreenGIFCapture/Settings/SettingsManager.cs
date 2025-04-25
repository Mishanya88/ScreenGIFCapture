using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Xml;
using GifLibrary;
using Newtonsoft.Json;
using ScreenGIFCapture.Controls;
using ScreenGIFCapture.ViewModels;
using static ScreenGIFCapture.Controls.HotkeyRecorder;

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
                SelectedCodec = viewModel.SelectedCodec.ToString(),
                FilePath = viewModel.FilePath,
                RegionHotkey = viewModel.RegionHotkey,
                FullScreenHotkey = viewModel.FullScreenHotkey,
                PauseHotkey = viewModel.PauseHotkey
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

                if (settings.RegionHotkey.Key == 0 && settings.RegionHotkey.Modifiers == 0)
                { 
                    settings.RegionHotkey = new RecordedHotkey
                    {
                        Key = (int)System.Windows.Forms.Keys.F2,
                        Modifiers = (int)WindowsHotkeys.KeyModifier.Alt
                    };
                }

                if (settings.FullScreenHotkey.Key == 0 && settings.FullScreenHotkey.Modifiers == 0)
                {
                    settings.FullScreenHotkey = new RecordedHotkey
                    {
                        Key = (int)System.Windows.Forms.Keys.S,
                        Modifiers = (int)WindowsHotkeys.KeyModifier.Ctrl
                    };
                }

                if (settings.PauseHotkey.Key == 0 && settings.PauseHotkey.Modifiers == 0)
                {
                    settings.PauseHotkey = new RecordedHotkey
                    {
                        Key = (int)System.Windows.Forms.Keys.F3,
                        Modifiers = (int)WindowsHotkeys.KeyModifier.Win
                    };
                }

                if (settings.Fps <= 0)
                {
                    settings.Fps = 10;
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
                return CreateDefaultSettings();
            }
        }

        public static string GetDefaultSavePath()
        {
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string savePath = Path.Combine(picturesFolder, "ScreenGIF");

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            return savePath;
        }

        private static SettingsModel CreateDefaultSettings()
        {
            return new SettingsModel
            {
                Fps = 10,
                SelectedCodec = GifQuality.Bit8.ToString(),
                FilePath = GetDefaultSavePath(),
                RegionHotkey = new RecordedHotkey
                {
                    Key = (int)Key.S,
                    Modifiers = (int)WindowsHotkeys.KeyModifier.Ctrl
                },
                FullScreenHotkey = new RecordedHotkey
                {
                    Key = (int)Key.F,
                    Modifiers = (int)WindowsHotkeys.KeyModifier.Ctrl
                },
                PauseHotkey = new RecordedHotkey
                {
                    Key = (int)Key.S,
                    Modifiers = (int)WindowsHotkeys.KeyModifier.Win
                }
            };
        }
    }
}