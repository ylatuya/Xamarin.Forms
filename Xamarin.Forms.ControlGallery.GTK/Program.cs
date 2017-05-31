using GLib;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms.Platform.GTK;

namespace Xamarin.Forms.ControlGallery.GTK
{
    class Program
    {
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += OnUnhandledException;

            Gtk.Application.Init();
            Forms.Init(new List<Assembly>
            {
                typeof(GtkToolkit.Controls.GridSplitter).Assembly,
                typeof(GtkToolkit.GTK.Renderers.GridSplitterRenderer).Assembly
            });

            var app = new Controls.App();
            var window = new FormsWindow();
            window.LoadApplication(app);

            window.Show();
            Gtk.Application.Run();
        }

        private static void OnUnhandledException(UnhandledExceptionArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled GTK# exception: {args.ExceptionObject}");
        }
    }
}