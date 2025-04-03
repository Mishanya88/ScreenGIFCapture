namespace ScreenGIFCapture.Base
{
    using System.Drawing;

    public interface IScreen
    {
        string Name { get; }

        Rectangle Rectangle { get; }
    }
}
