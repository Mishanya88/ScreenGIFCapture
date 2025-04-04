namespace ScreenGIFCapture.Service
{

    public static class ServiceProvider
    {
        public static readonly IServices ServicesPlatform;

        static ServiceProvider()
        {
            ServicesPlatform = new WindowsServices();
        }
    }
}
