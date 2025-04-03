namespace ScreenGIFCapture.Screen
{
    using System.Linq;
    using ScreenGIFCapture.Base;
    using ScreenGIFCapture.Service;

    public static class ScreenWindow
    {

        public static IScreen GetScreen()
        {
            var platformServices = ServiceProvider.ServicesPlatform;
            return platformServices.EnumerateScreens().FirstOrDefault();
        }


    }
}
