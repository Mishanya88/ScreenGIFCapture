using System;
using System.IO;
using GifLibrary;
using ScreenGIFCapture.Controls;

namespace ScreenGIFCapture.Settings
{
    public class SettingsModel
    {
        public int Fps { get; set; }
        public string SelectedCodec { get; set; } = GifQuality.Bit8.ToString();
        public string FilePath { get; set; }
        public RecordedHotkey RegionHotkey { get; set; }
        public RecordedHotkey FullScreenHotkey { get; set; }
        public RecordedHotkey PauseHotkey { get; set; }
        public GifQuality GetCodec() =>
            Enum.TryParse<GifQuality>(SelectedCodec, out var codec) ? codec : GifQuality.Bit8;
    }
}