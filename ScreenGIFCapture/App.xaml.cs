using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading;
using System.Windows;
using ScreenGIFCapture.Service;

namespace ScreenGIFCapture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _registered = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            //#region terminate if there's any existing instance
            //string appName = System.IO.Path.GetFileNameWithoutExtension(
            //    System.Reflection.Assembly.GetEntryAssembly().Location);

            //Mutex procMutex = new System.Threading.Mutex(true, $"_{appName.ToUpper()}", out bool result);
            //if (!result)
            //{
            //    Application.Current.Shutdown(0);
            //    return;
            //}

            //procMutex.ReleaseMutex();

            //#endregion

            // click on the notification tip
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                // Obtain the arguments from the notification
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                // Need to dispatch to UI thread if performing UI operations
                Application.Current.Dispatcher.Invoke(delegate
                {
                    if (args.Contains("imagePath"))
                    {
                        string s = args.Get("imagePath");
                        NotificationsService.OpenFile(s);
                    }
                });
            };
            _registered = true;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_registered)
            {
                //unload notifications
                ToastNotificationManagerCompat.Uninstall();
            }
        }
    }
}
