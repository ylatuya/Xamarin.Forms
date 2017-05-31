using GtkToolkit.Controls;
using GtkToolkit.GTK.Renderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace GtkToolkit.GTK.Renderers
{
    public class SeparatorRenderer : ViewRenderer<Separator, Gtk.Separator>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Separator> e)
        {
            if (Control == null)
            {
                SetNativeControl(new Gtk.Separator(IntPtr.Zero));
            }

            base.OnElementChanged(e);
        }
    }
}