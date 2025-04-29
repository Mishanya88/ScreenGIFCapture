using System;
using System.Windows.Controls;
using GifLibrary;

namespace ScreenGIFCapture.Settings
{
    public class SettingsModel
    {
        public int Fps { get; set; }
        public string SelectedCodec { get; set; } = GifQuality.Bit8.ToString();
        public string FilePath { get; set; }
        public RecordedHotkey RegionHotkey { get; set; }
        public RecordedHotkey FullScreenHotkey { get; set; }
        public RecordedHotkey RecordWindowHotkey { get; set; }
        public RecordedHotkey PauseHotkey { get; set; }
        public string EncryptedPassword { get; set; }
        public string BodyEmail { get; set; }
        public string Subject { get; set; }
        public string SenderEmail { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpServer { get; set; }
        public string ToAddress { get; set; }
        public GifQuality GetCodec() =>
            Enum.TryParse<GifQuality>(SelectedCodec, out var codec) ? codec : GifQuality.Bit8;
    }
}