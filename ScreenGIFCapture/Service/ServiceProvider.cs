namespace ScreenGIFCapture.Service
{

    public static class ServiceProvider
    {
        public static readonly IServices IServicesPlatform;

        static ServiceProvider()
        {
            IServicesPlatform = new WindowsServices();
        }
    }
}
