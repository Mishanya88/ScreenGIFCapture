using System;
using GifLibrary;

namespace ScreenGIFCapture.Settings
{
    public class SettingsModel
    {
        public int Fps { get; set; }
        public string SelectedCodec { get; set; } = GifQuality.Bit8.ToString();

        public GifQuality GetCodec() =>
            Enum.TryParse<GifQuality>(SelectedCodec, out var codec) ? codec : GifQuality.Bit8;
    }
}