using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenGIFCapture.Gif
{
    internal interface IImageProvider : IDisposable
    {
        Bitmap Capture();

        int Height { get; }

        int Width { get; }
    }
}
