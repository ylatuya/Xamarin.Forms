using GLib;
using Xamarin.Forms.Platform.GTK;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.ControlGallery.GTK
{
    class Program
    {
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += OnUnhandledException;

            Gtk.Application.Init();
            Forms.Init();

            var app = new Controls.App();
            var window = new FormsWindow();
            window.LoadApplication(app);

            window.Show();

            Gtk.Application.Run();
        }

        private static void OnUnhandledException(UnhandledExceptionArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled GTK# exception: {args}");
        }
    }
}